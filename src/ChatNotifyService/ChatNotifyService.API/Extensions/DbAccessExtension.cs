using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.API.Extensions;

/// <summary>
/// Extension methods for configuring database access services in the WebApplicationBuilder.
/// </summary>
public static class DbAccessExtension
{
    /// <summary>
    /// Configures the application's DbContext and related services for database access.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<ChatNotifyDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<PermissionLevel>(schemaName: "public", enumName:"permission_level");
            });
        });

        return builder;
    }

    /// <summary>
    /// Registers the application's data models as scoped services in the dependency injection container.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<Chat>();
        builder.Services.AddScoped<Message>();
        builder.Services.AddScoped<ChatMember>();
        builder.Services.AddScoped<MessageRead>();
        builder.Services.AddScoped<SpaceActivityLog>();
        
        return builder;
    }

    /// <summary>
    /// Registers the application's repository interfaces and their implementations as scoped services in the dependency injection container.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IMessageReadRepository, MessageReadRepository>();
        builder.Services.AddScoped<ISpaceActivityLogRepository, SpaceActivityLogRepository>();
        
        return builder;
    }
}
