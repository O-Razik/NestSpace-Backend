using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Mappers;
using EventScheduleService.BLL.Mappers.Send;
using EventScheduleService.BLL.Services;
using EventScheduleService.BLL.Validators;
using FluentValidation;

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
    /// Registers the application's mappers with the dependency injection container
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<RegularEventMapper>();
        builder.Services.AddScoped<TagMapper>();
        builder.Services.AddScoped<CategoryMapper>();
        builder.Services.AddScoped<SoloEventMapper>();

        builder.Services.AddScoped<IMapper<EventCategory, CategoryDto>, CategoryMapper>();
        builder.Services.AddScoped<IMapper<EventCategory, CategoryShortDto>, CategoryShortMapper>();
        builder.Services.AddScoped<IMapper<EventTag, TagDto>, TagMapper>();
        builder.Services.AddScoped<IMapper<RegularEvent, RegularEventDto>, RegularEventMapper>();
        builder.Services.AddScoped<IMapper<SoloEvent, SoloEventDto>, SoloEventMapper>();
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
        builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return builder;
    }

    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
            builder.Services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateTagDto>, CreateTagDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateSoloEventDto>, CreateSoloEventDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateRegularEventDto>, CreateRegularEventDtoValidator>();
    
            builder.Services.AddScoped<IValidator<UpdateSoloEventDto>, UpdateSoloEventDtoValidator>();
            builder.Services.AddScoped<IValidator<UpdateRegularEventDto>, UpdateRegularEventDtoValidator>();
    
            return builder;
    }
}
