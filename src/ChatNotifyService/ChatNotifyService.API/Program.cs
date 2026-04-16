using System.Diagnostics;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.API.Extensions;
using ChatNotifyService.API.Middleware;

namespace ChatNotifyService.API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var serviceName = builder.Environment.ApplicationName;
        var otlpEndpoint = builder.Configuration["OTEL_SERVICE_NAME"] ?? "not-configured";

        builder.Services.AddControllers();
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();

        builder.AddSqlDbContext()
            .AddRabbitMqServices()
            .AddModels()
            .AddRepositories()
            .AddDateTimeProvider()
            .AddServices()
            .AddMappers()
            .AddSerilog()
            .AddOpenTelemetry();
        
        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwagger();
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapGet("/health", () =>
        {
            using var healthActivity = new ActivitySource("HealthCheck").StartActivity("HealthCheck");
            healthActivity?.SetTag("health.status", "healthy");
            IDateTimeProvider dateTimeProvider = app.Services.GetRequiredService<IDateTimeProvider>();
            
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
