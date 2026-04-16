using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Helpers;
using ChatNotifyService.BLL.Services;

namespace ChatNotifyService.API.Extensions;

/// <summary>
/// Extension methods for configuring business logic services and mappers in the WebApplicationBuilder.
/// </summary>
public static class BusinessLogicExtension
{
    
    /// <summary>
    /// Adds business logic services to the WebApplicationBuilder's service collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IChatNotificationService, ChatNotificationService>();
        builder.Services.AddScoped<ISpaceActivityLogService, SpaceActivityLogService>();
        builder.Services.AddScoped<SpaceActivityLogHelper>();
        
        return builder;
    }
}
