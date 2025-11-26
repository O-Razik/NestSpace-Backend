using Ocelot.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace Gateway.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("GatewayAPI"))
            .WithMetrics(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            )
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            );

        builder.Logging.AddOpenTelemetry(logging => logging
            .AddOtlpExporter()
        );
        
        return builder;
    }
    
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("Microservice", "Notifications")
            .WriteTo.Console()
            .WriteTo.OpenTelemetry()
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}