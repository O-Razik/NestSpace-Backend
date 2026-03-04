using System.Diagnostics;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.API.Extensions;
using UserSpaceService.API.Filters;
using UserSpaceService.API.Middleware;

namespace UserSpaceService.API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var serviceName = builder.Environment.ApplicationName;
        var otlpEndpoint = builder.Configuration["OTEL_SERVICE_NAME"] ?? "not-configured";

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<FluentValidationFilter>();
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerWithJwtAuth();
        

        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.AddSqlDbContext()
            .AddModels()
            .AddRabbitMqServices()
            .AddRepositories()
            .AddServices()
            .AddMappersAndFactories()
            .AddValidation()
            .AddSerilog()
            .AddOpenTelemetry();
        
        builder.Services.AddAuthorization();
        builder.Services.AddAllAuthentication(builder.Configuration);
        builder.Services.AddHttpContextAccessor();

        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapGet("/health", (IDateTimeProvider dateTimeProvider) =>
        {
            using var healthActivity = new ActivitySource("HealthCheck").StartActivity("HealthCheck");
            healthActivity?.SetTag("health.status", "healthy");

            return Results.Ok(new
            {
                status = "healthy",
                service = serviceName,
                timestamp = dateTimeProvider.UtcNow,
                otlpEndpoint
            });
        }).WithName("HealthCheck").WithOpenApi();

        app.Run();
    }
}
