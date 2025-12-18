using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Dto.Create;
using EventScheduleService.BLL.Dto.Send;
using EventScheduleService.BLL.Dto.Update;
using EventScheduleService.BLL.Mappers.Create;
using EventScheduleService.BLL.Mappers.Send;
using EventScheduleService.BLL.Mappers.Update;
using EventScheduleService.BLL.RabbitMQ;
using EventScheduleService.BLL.RabbitMQ.Consumer;
using EventScheduleService.BLL.RabbitMQ.Publisher;
using EventScheduleService.BLL.Services;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Factories;
using EventScheduleService.DAL.Models;
using EventScheduleService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace EventScheduleService.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<EventScheduleDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<Day>("day");
                npgsqlOptions.MapEnum<Frequency>("frequency");
            }));
        return builder;
    }
    
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventCategory, EventCategory>();
        builder.Services.AddScoped<ISoloEvent, SoloEvent>();
        builder.Services.AddScoped<IEventTag, EventTag>();
        builder.Services.AddScoped<IRegularEvent, RegularEvent>();
        return builder;
    }
    
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
        builder.Services.AddScoped<ISoloEventRepository, SoloEventRepository>();
        builder.Services.AddScoped<IEventTagRepository, EventTagRepository>();
        builder.Services.AddScoped<IRegularEventRepository, RegularEventRepository>();
        return builder;
    }
    
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddScoped<ISoloEventService, SoloEventService>();
        builder.Services.AddScoped<IRegularEventService, RegularEventService>();
        return builder;
    }
    
    public static WebApplicationBuilder AddMappersAndFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEntityFactory<IEventCategory>, CategoryFactory>();
        builder.Services.AddScoped<IEntityFactory<IEventTag>,TagFactory>();
        builder.Services.AddScoped<IEntityFactory<ISoloEvent>, SoloEventFactory>();
        builder.Services.AddScoped<IEntityFactory<IRegularEvent>, RegularEventFactory>();

        builder.Services.AddScoped<RegularEventMapper>();
        builder.Services.AddScoped<TagMapper>();
        builder.Services.AddScoped<CategoryMapper>();
        builder.Services.AddScoped<SoloEventMapper>();

        builder.Services.AddScoped<IMapper<IEventCategory, CategoryDto, CategoryShortDto>, CategoryMapper>();
        builder.Services.AddScoped<IMapper<IEventTag, TagDto>, TagMapper>();
        builder.Services.AddScoped<IMapper<IRegularEvent, RegularEventDto>, RegularEventMapper>();
        builder.Services.AddScoped<IMapper<ISoloEvent, SoloEventDto>, SoloEventMapper>();

        builder.Services.AddScoped<ICreateMapper<IEventCategory, CategoryCreateDto>, CategoryCreateMapper>();
        builder.Services.AddScoped<ICreateMapper<IEventTag, TagCreateDto>, TagCreateMapper>();
        builder.Services.AddScoped<ICreateMapper<ISoloEvent, SoloEventCreateDto>, SoloEventCreateMapper>();
        builder.Services.AddScoped<ICreateMapper<IRegularEvent, RegularEventCreateDto>, RegularEventCreateMapper>();

        builder.Services.AddScoped<ICreateMapper<ISoloEvent, SoloEventUpdateDto>, SoloEventUpdateMapper>();
        builder.Services.AddScoped<ICreateMapper<IRegularEvent, RegularEventUpdateDto>, RegularEventUpdateMapper>();

        return builder;
    }
    
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        builder.Services.AddSingleton<RabbitMqConsumer>();
        builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
        
        return builder;
    }
    
    /*
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console()
            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
            {
                clientConfiguration.Username = "guest";
                clientConfiguration.Password = "guest";
                clientConfiguration.VHost = "/";
                clientConfiguration.Hostnames.Add("localhost");
                clientConfiguration.Exchange = "logs_exchange";
                clientConfiguration.ExchangeType = "direct";
                sinkConfiguration.TextFormatter = new JsonFormatter();
            })
            .CreateLogger();

        builder.Host.UseSerilog();
        
        return builder;
    }
    */
    
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
                    options.Filter = (httpContext) => 
                    {
                        return !httpContext.Request.Path.Value?.Contains("/health") ?? true;
                    };
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