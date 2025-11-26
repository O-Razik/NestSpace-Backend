using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos;
using ChatNotifyService.BLL.Mappers;
using ChatNotifyService.BLL.RabbitMQ;
using ChatNotifyService.BLL.Services;
using ChatNotifyService.DAL.Entities;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Factories;
using ChatNotifyService.DAL.Repositories;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Events;

namespace ChatNotifyService.API.Extensions;

public static class BuilderExtension
{
    public static WebApplicationBuilder AddNoSqlDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ChatNotifyDbContext>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseName = configuration["DatabaseSettings:DatabaseName"];
            return new ChatNotifyDbContext(connectionString, databaseName);
        });

        return builder;
    }
    
    public static async Task InitializeMongoIndexesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatNotifyDbContext>();
        await dbContext.InitializeIndexesAsync();
    }
    
    public static WebApplicationBuilder AddQueues(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
        builder.Configuration.GetSection("RabbitSettings"));
         
        builder.Services.AddSingleton<RabbitMqConsumer>();
        builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
        
        return builder;
    }

    public static WebApplicationBuilder AddEntities(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChat, Chat>();
        builder.Services.AddScoped<IMessage, Message>();
        builder.Services.AddScoped<IChatMember, ChatMember>();
        builder.Services.AddScoped<IMessageRead, MessageRead>();
        
        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IMessageReadRepository, MessageReadRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IChatNotificationService, ChatNotificationService>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEntityFactory<IChat>, ChatFactory>();
        builder.Services.AddTransient<IEntityFactory<IMessage>, MessageFactory>();
        builder.Services.AddTransient<IEntityFactory<IChatMember>, ChatMemberFactory>();
        builder.Services.AddTransient<IEntityFactory<IMessageRead>, MessageReadFactory>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ChatMemberMapper>();
        builder.Services.AddTransient<MessageReadMapper>();
        builder.Services.AddTransient<ChatMapper>();
        builder.Services.AddTransient<IBigMapper<IChat, ChatDto, ChatDtoShort>, ChatMapper>();
        builder.Services.AddTransient<IBigMapper<IMessage, MessageDto, MessageDtoShort>, MessageMapper>();
        builder.Services.AddTransient<IBigMapper<IChatMember, ChatMemberDto, ChatMemberDtoShort>, ChatMemberMapper>();
        builder.Services.AddTransient<IBigMapper<IMessageRead, MessageReadDto, MessageReadDtoShort>, MessageReadMapper>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService("ChatNotifyService"))  // service name (required)
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("NestSpace")               // custom ActivitySource
                .AddOtlpExporter()
            )
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter()
            );

        builder.Logging.AddOpenTelemetry(logging 
            => logging.AddOtlpExporter());
        
        return builder;
    }
    
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
            
        var mongoConnectionString = builder.Configuration.GetSection("DefaultConnection").Value;
        var mongoDatabaseName = builder.Configuration.GetSection("DatabaseSettings:DatabaseName").Value;
        
        var mongoUrlBuilder = new MongoUrlBuilder(mongoConnectionString);
        if (string.IsNullOrEmpty(mongoUrlBuilder.DatabaseName))
        {
            mongoUrlBuilder.DatabaseName = mongoDatabaseName;
        }
        var finalMongoUrl = mongoUrlBuilder.ToString();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("Microservice", "Notifications")
            .WriteTo.Console()
            .WriteTo.MongoDB(
                databaseUrl: finalMongoUrl,
                collectionName: "Logs",
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.OpenTelemetry()
            .CreateLogger();
        
        builder.Host.UseSerilog();
        
        return builder;
    }

    
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSignalR();
        
        return builder
            .AddNoSqlDbContext()
            .AddEntities()
            .AddRepositories()
            .AddServices()
            .AddFactories()
            .AddMappers()
            .AddQueues()
            .AddOpenTelemetry();
    }
}