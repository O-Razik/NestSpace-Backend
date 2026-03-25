using System.Diagnostics;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.API.Extensions;
using EventScheduleService.API.Filters;
using EventScheduleService.API.Middleware;

namespace EventScheduleService.API;

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
        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder
            .AddSqlDbContext()
            .AddModels()
            .AddRepositories()
            .AddServices()
            .AddMappersAndFactories()
            .AddRabbitMqServices()
            .AddSerilog()
            .AddOpenTelemetry();

        builder.Services
            .AddAuthorization()
            .AddEndpointsApiExplorer();

        builder.Services.AddSwagger();

        var app = builder.Build();

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

        app.MapGet("/health", () =>
        {
            using var healthActivity = new ActivitySource("HealthCheck").StartActivity("HealthCheck");
            var dateTimeProvider = app.Services.GetRequiredService<IDateTimeProvider>();
            healthActivity?.SetTag("health.status", "healthy");
            
            return Results.Ok(new { 
                status = "healthy", 
                service = serviceName, 
                timestamp = dateTimeProvider.UtcNow, 
                otlpEndpoint }); 
        }).WithName("HealthCheck").WithOpenApi();
        
        app.Run();
    }
}
