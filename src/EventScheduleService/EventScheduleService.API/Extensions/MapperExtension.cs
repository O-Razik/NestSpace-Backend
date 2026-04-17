using EventScheduleService.ABS.Dtos;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.BLL.Mappers;
using EventScheduleService.BLL.Mappers.Send;
using EventScheduleService.BLL.Validators;
using FluentValidation;

namespace EventScheduleService.API.Extensions;

/// <summary>
/// The MapperExtension class provides extension methods for configuring the application's mappers in the dependency injection container,
/// allowing for easy mapping of data between different layers of the application, such as from database models to API response DTOs and from API request DTOs to database models.
/// </summary>
public static class MapperExtension
{
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
        
        builder.Services.AddScoped<IEntityMapper<RegularEvent, RegularEventCreateDto>, RegularEventMapper>();
        builder.Services.AddScoped<IEntityMapper<SoloEvent, SoloEventCreateDto>, SoloEventMapper>();

        return builder;
    }
    
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
        builder.Services.AddScoped<IValidator<CreateTagDto>, TagCreateDtoValidator>();
        builder.Services.AddScoped<IValidator<SoloEventCreateDto>, SoloEventDtoCreateValidator>();
        builder.Services.AddScoped<IValidator<RegularEventCreateDto>, RegularEventCreateDtoValidator>();
    
        builder.Services.AddScoped<IValidator<SoloEventUpdateDto>, SoloEventUpdateDtoValidator>();
        builder.Services.AddScoped<IValidator<UpdateRegularEventDto>, RegularEventUpdateDtoValidator>();
    
        return builder;
    }
    
}
