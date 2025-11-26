using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.DTOs;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Mappers;
using UserSpaceService.BLL.Queues;
using UserSpaceService.BLL.Services;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Helpers;
using UserSpaceService.DAL.Models;
using UserSpaceService.DAL.Repositories;

namespace UserSpaceService.API.Extensions;

public static class BuilderExtension
{
    public static WebApplicationBuilder AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<UserSpaceDbContext>(options => options.UseNpgsql(connectionString));
        return builder;
    }
    
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IExternalLogin, ExternalLogin>();
        builder.Services.AddScoped<ISpaceRole, SpaceRole>();
        builder.Services.AddScoped<IUser, User>();
        builder.Services.AddScoped<ISpaceMember, SpaceMember>();
        builder.Services.AddScoped<ISpace, Space>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISpaceRoleRepository, SpaceRoleRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
        builder.Services.AddScoped<ISpaceMemberRepository, SpaceMemberRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISpaceService, SpaceService>();
        
        builder.Services.AddScoped<IPasswordHasher<IUser>, PasswordHasher<IUser>>();
        builder.Services.AddScoped<PasswordHasher<IUser>>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<GoogleTokenValidator>();
        builder.Services.AddScoped<MicrosoftTokenValidator>();
        builder.Services.AddScoped<ExternalTokenValidatorFactory>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddMappersAndFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEntityFactory<IUser>, UserFactory>();
        builder.Services.AddScoped<IEntityFactory<IExternalLogin>, ExternalLoginFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpace>, SpaceFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpaceMember>, SpaceMemberFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpaceRole>, SpaceRoleFactory>();
    
        builder.Services.AddScoped<IMapper<IUser, UserDto>, UserMapper>();
        builder.Services.AddScoped<IMapper<IUser, UserDtoShort>, UserShortMapper>();
        builder.Services.AddScoped<IMapper<ISpaceMember, SpaceMemberDto>, SpaceMemberMapper>();
        builder.Services.AddScoped<IMapper<ISpaceMember, SpaceMemberDtoShort>, SpaceMemberShortMapper>();
        builder.Services.AddScoped<IMapper<ISpace, SpaceDto>, SpaceMapper>();
        builder.Services.AddScoped<IMapper<ISpace, SpaceDtoShort>, SpaceShortMapper>();
        builder.Services.AddScoped<IMapper<ISpaceRole, SpaceRoleDto>, SpaceRoleMapper>();
        builder.Services.AddScoped<IMapper<IExternalLogin, ExternalLoginDto>, ExternalLoginMapper>(); 
        
        return builder;
    }
    
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService("UserSpaceService"))  // service name (required)
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

        var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");

        // Load Rabbit settings
        var rabbit = builder.Configuration.GetSection("RabbitSettings");

        var rabbitHost = rabbit["HostName"];
        var rabbitPort = int.Parse(rabbit["Port"] ?? "5672");
        var rabbitUser = rabbit["UserName"];
        var rabbitPass = rabbit["Password"];

        // PostgreSQL log column options
        var columnOptions = new Dictionary<string, ColumnWriterBase>
        {
        { "message", new RenderedMessageColumnWriter() },
        { "message_template", new MessageTemplateColumnWriter() },
        { "level", new LevelColumnWriter(renderAsText: true) },
        { "timestamp", new TimestampColumnWriter() },
        { "exception", new ExceptionColumnWriter() },
        { "properties", new LogEventSerializedColumnWriter() }
        };

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.WithProperty("Microservice", "Notifications")
        .WriteTo.Console()
        .WriteTo.PostgreSQL(
            connectionString: postgresConnectionString,
            tableName: "Logs",
            columnOptions: columnOptions,
            needAutoCreateTable: true,
            restrictedToMinimumLevel: LogEventLevel.Information
        )
        // RabbitMQ logging with correct property names
        .WriteTo.RabbitMQ((clientConfig, sinkConfig) =>
        {
            clientConfig.Hostnames.Add(rabbitHost);
            clientConfig.Port = rabbitPort;
            clientConfig.Username = rabbitUser;
            clientConfig.Password = rabbitPass;
            sinkConfig.RestrictedToMinimumLevel = LogEventLevel.Information;
        })
        .WriteTo.OpenTelemetry()
        .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    
    public static WebApplicationBuilder AddQueueServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        
        return builder;
    }
}