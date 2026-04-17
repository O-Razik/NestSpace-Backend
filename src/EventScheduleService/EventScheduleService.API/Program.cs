using System.Diagnostics;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.API.Extensions;
using EventScheduleService.API.Filters;
using EventScheduleService.API.Middleware;

namespace EventScheduleService.API;

/// <summary>
/// The main entry point of the Event Schedule Service API application.
/// This class is responsible for configuring and running the web application, including setting up services,
/// middleware, and defining the HTTP request pipeline.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point of the application. It sets up the web application, configures services, and defines the HTTP request pipeline.
    /// </summary>
    /// <param name="args"></param>
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
            .AddHelpers()
            .AddRabbitMqServices()
            .AddServices()
            .AddMappers()
            .AddValidation()
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
