using EventScheduleService.ABS.IHelpers;
using EventScheduleService.BLL.RabbitMQ;
using EventScheduleService.BLL.RabbitMQ.Consumer;
using EventScheduleService.BLL.RabbitMQ.Publisher;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace EventScheduleService.API.Extensions;

/// <summary>
/// The MicroserviceExtension class provides extension methods for configuring microservice-related services and features in the application,
/// such as RabbitMQ integration, Serilog logging, and OpenTelemetry observability.
/// </summary>
public static class MicroserviceExtension
{
    
    /// <summary>
    /// Registers the application's RabbitMQ services with the dependency injection container,
    /// allowing them to be injected into services and controllers as needed.
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure RabbitMQ services for.</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        builder.Services.AddSingleton<RabbitMqConsumer>();
        builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
        builder.Services.AddScoped<SpaceLogPublish>();
        
        return builder;
    }
    
    /// <summary>
    /// Configures Serilog as the logging provider for the application, reading configuration from the application's configuration sources
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure Serilog for.</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "EventScheduleService")
        );
        
        return builder;
    }
    
    /// <summary>
    /// Configures OpenTelemetry for the application, including resource attributes, metrics, tracing, and logging.
    /// The configuration is based on environment variables and application settings, allowing for flexible deployment and observability.
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure OpenTelemetry for.</param>
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
                    if (string.IsNullOrEmpty(otlpEndpoint))
                    {
                        return;
                    }
                    options.Endpoint = new Uri(otlpEndpoint);
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = (httpContext) => !httpContext.Request.Path.Value?.Contains("/health") ?? true;
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("EventScheduleService.*")
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
