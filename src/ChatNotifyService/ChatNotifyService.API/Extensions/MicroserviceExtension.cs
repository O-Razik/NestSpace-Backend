using ChatNotifyService.BLL.RabbitMQ;
using ChatNotifyService.BLL.RabbitMQ.Consumer;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace ChatNotifyService.API.Extensions;

/// <summary>
/// Extension methods for configuring microservice-related services such as RabbitMQ, Serilog, and OpenTelemetry in the WebApplicationBuilder.
/// </summary>
public static class MicroserviceExtension
{
    /// <summary>
    /// Configures RabbitMQ-related services, including settings, consumer, and hosted service for consuming messages from RabbitMQ.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));
         
        builder.Services.AddSingleton<RabbitMqConsumer>();
        builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
        
        return builder;
    }
    
    /// <summary>
    /// Configures Serilog as the logging provider, reading settings from configuration and enriching logs with contextual information such as thread ID, environment name, and application name.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "ChatNotifyService")
        );
        
        return builder;
    }
    
    /// <summary>
    /// Configures OpenTelemetry for metrics, tracing, and logging, including resource attributes,
    /// instrumentation for ASP.NET Core and HTTP clients, and exporting to an OTLP endpoint if configured.
    /// It also sets up a filter to exclude health check endpoints from tracing and configures the sampler to always sample.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "EventScheduleService";
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName,
                    ["service.namespace"] = "NestSpace"
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(options =>
                {
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    }
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = httpContext =>
                        !(httpContext.Request.Path.Value?.Contains("/health", StringComparison.OrdinalIgnoreCase) ?? false);
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("ChatNotifyService.*")
                .AddSource("RabbitMQ.*")
                .SetSampler(new AlwaysOnSampler())
                .AddOtlpExporter(options =>
                {
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    }
                }));

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
            
            logging.AddOtlpExporter(options =>
            {
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }
            });
        });
        
        return builder;
    }
}
