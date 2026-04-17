using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.API.Extensions;

/// <summary>
/// Extension methods for configuring data provider services in the WebApplicationBuilder,
/// allowing for easy injection of data-related services such as date and time providers into controllers and other services throughout the application.
/// </summary>
public static class DataProviderExtention
{
    /// <summary>
    /// Adds a singleton instance of the IDateTimeProvider service to the WebApplicationBuilder's service collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddDateTimeProvider(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return builder;
    }
}
