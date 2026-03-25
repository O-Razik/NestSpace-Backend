using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.API.Extensions;

/// <summary>
/// Provides extension methods for configuring the application's database context,
/// models, and repositories with the dependency injection container.
/// </summary>
public static class DbAccessExtension
{
    /// <summary>
    /// Configures the application's DbContext with PostgreSQL and maps the Day and Frequency enums to PostgreSQL enum types.
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure the DbContext for.</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<EventScheduleDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<Day>("day");
                npgsqlOptions.MapEnum<Frequency>("frequency");
            }));
        return builder;
    }
    
    /// <summary>
    /// Registers the application's models with the dependency injection container,
    /// allowing them to be injected into services and controllers as needed.
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure models for.</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<EventCategory>();
        builder.Services.AddScoped<SoloEvent>();
        builder.Services.AddScoped<EventTag>();
        builder.Services.AddScoped<RegularEvent>();
        return builder;
    }
    
    /// <summary>
    /// Registers the application's repositories with the dependency injection container,
    /// allowing them to be injected into services and controllers as needed.
    /// </summary>
    /// <param name="builder"> The WebApplicationBuilder to configure repositories for.</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
        builder.Services.AddScoped<ISoloEventRepository, SoloEventRepository>();
        builder.Services.AddScoped<IEventTagRepository, EventTagRepository>();
        builder.Services.AddScoped<IRegularEventRepository, RegularEventRepository>();
        return builder;
    }
}
