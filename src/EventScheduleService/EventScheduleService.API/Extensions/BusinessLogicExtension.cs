using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Services;

namespace EventScheduleService.API.Extensions;

/// <summary>
/// The BusinessLogicExtension class provides extension methods for configuring
/// the application's business logic services, mappers, and factories in the dependency injection container.
/// </summary>
public static class BusinessLogicExtension
{
    
    /// <summary>
    /// Registers the application's services with the dependency injection container, allowing them to be injected into controllers as needed.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddScoped<ISoloEventService, SoloEventService>();
        builder.Services.AddScoped<IRegularEventService, RegularEventService>();
        return builder;
    }
    
    /// <summary>
    /// Registers the application's helper services with the dependency injection container,
    /// allowing them to be injected into services and controllers as needed.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddHelpers(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return builder;
    }
}
