using System.Diagnostics;
using Ocelot.Middleware;

namespace NestSpace_Gateway;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var serviceName = builder.Environment.ApplicationName;
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "not-configured";
        
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.AddOcelot().AddSerilog().AddOpenTelemetry();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseCors("AllowAll");
        
        app.MapGet("/health", () =>
        {
            using var healthActivity = new ActivitySource("HealthCheck").StartActivity("HealthCheck");
            healthActivity?.SetTag("health.status", "healthy");

            return Results.Ok(new
            {
                status = "healthy",
                service = serviceName,
                timestamp = DateTime.UtcNow,
                otlpEndpoint = otlpEndpoint
            });
        }).WithName("HealthCheck").WithOpenApi();

        await app.UseOcelot();
        await app.RunAsync();
    }
}