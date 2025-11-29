using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Dto.Create;
using EventScheduleService.BLL.Dto.Send;
using EventScheduleService.BLL.Mappers.Create;
using EventScheduleService.BLL.Mappers.Send;
using EventScheduleService.BLL.Services;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Factories;
using EventScheduleService.DAL.Models;
using EventScheduleService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.API.Extensions;

public static class BuilderExtensions
{
    public static void AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<EventScheduleDbContext>(options => options.UseNpgsql(connectionString));
    }
    
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventCategory, EventCategory>();
        builder.Services.AddScoped<ISoloEvent, SoloEvent>();
        builder.Services.AddScoped<IEventTag, EventTag>();
        builder.Services.AddScoped<IRegularEvent, RegularEvent>();
        return builder;
    }
    
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
        builder.Services.AddScoped<ISoloEventRepository, SoloEventRepository>();
        builder.Services.AddScoped<IEventTagRepository, EventTagRepository>();
        builder.Services.AddScoped<IRegularEventRepository, RegularEventRepository>();
        return builder;
    }
    
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddScoped<ISoloEventService, SoloEventService>();
        builder.Services.AddScoped<IRegularEventService, RegularEventService>();
        return builder;
    }
    
    public static WebApplicationBuilder AddMappersAndFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEntityFactory<IEventCategory>, CategoryFactory>();
        builder.Services.AddScoped<IEntityFactory<IEventTag>,TagFactory>();
        builder.Services.AddScoped<IEntityFactory<ISoloEvent>, SoloEventFactory>();
        builder.Services.AddScoped<IEntityFactory<IRegularEvent>, RegularEventFactory>();
        
        builder.Services.AddScoped<IMapper<IEventCategory, CategoryDto, CategoryShortDto>, CategoryMapper>();
        builder.Services.AddScoped<IMapper<IEventTag, TagDto>, TagMapper>();
        builder.Services.AddScoped<IMapper<IRegularEvent, RegularEventDto>, RegularEventMapper>();
        builder.Services.AddScoped<IMapper<ISoloEvent, SoloEventDto>, SoloEventMapper>();
        
        builder.Services.AddScoped<IShortMapper<IEventCategory, CategoryCreateDto>, CategoryCreateMapper>();
        builder.Services.AddScoped<IShortMapper<IEventTag, TagCreateDto>, TagCreateMapper>();
        builder.Services.AddScoped<IShortMapper<ISoloEvent, SoloEventCreateDto> , SoloEventCreateMapper>();
        builder.Services.AddScoped<IShortMapper<IRegularEvent, RegularEventCreateDto> , RegularEventCreateMapper>();
        
        builder.Services.AddScoped<IMapper<ISoloEvent, SoloEventDto>, SoloEventMapper>();
        return builder;
    }
    
}