using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.ABS.Mappers;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Queues;
using UserSpaceService.BLL.Services;
using UserSpaceService.BLL.Validators;
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
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<UserSpaceDbContext>(options => options.UseNpgsql(connectionString));
        
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
    
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<LoginDtoShort>, LoginDtoShortValidator>();
        builder.Services.AddScoped<IValidator<RegisterDtoShort>, RegisterDtoShortValidator>();
        builder.Services.AddScoped<IValidator<AddSpaceMemberDto>, AddSpaceMemberDtoValidator>();
        builder.Services.AddScoped<IValidator<CreateSpaceDto>, CreateSpaceDtoValidator>();
        builder.Services.AddScoped<IValidator<SpaceRoleDtoShort>, SpaceRoleDtoShortValidator>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "UserSpaceService")
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
                .AddSource("UserSpaceService.*")
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