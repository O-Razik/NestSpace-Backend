using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Json;
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
    public static void AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<UserSpaceDbContext>(options => options.UseNpgsql(connectionString));
    }
    
    public static void AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IExternalLogin, ExternalLogin>();
        builder.Services.AddScoped<ISpaceRole, SpaceRole>();
        builder.Services.AddScoped<IUser, User>();
        builder.Services.AddScoped<ISpaceMember, SpaceMember>();
        builder.Services.AddScoped<ISpace, Space>();
    }
    
    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISpaceRoleRepository, SpaceRoleRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
        builder.Services.AddScoped<ISpaceMemberRepository, SpaceMemberRepository>();
    }
    
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISpaceService, SpaceService>();
        
        builder.Services.AddScoped<IPasswordHasher<IUser>, PasswordHasher<IUser>>();
        builder.Services.AddScoped<PasswordHasher<IUser>>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<GoogleTokenValidator>();
        builder.Services.AddScoped<MicrosoftTokenValidator>();
        builder.Services.AddScoped<ExternalTokenValidatorFactory>();
    }
    
    public static void AddMappersAndFactories(this WebApplicationBuilder builder)
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
    }
    
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        
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
            .Enrich.WithProperty("Application", "UserSpaceService")
        );
        
        return builder;
    }

}