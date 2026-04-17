using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Mappers;

namespace ChatNotifyService.API.Extensions;

/// <summary>
/// Extension methods for configuring mappers in the WebApplicationBuilder,
/// allowing for easy mapping of data between different layers of the application, such as from database models to API response DTOs and from API request DTOs to database models.
/// </summary>
public static class MapperExtension
{
    /// <summary>
    /// Adds mappers to the WebApplicationBuilder's service collection, including both specific mappers for individual models and generic mappers for mapping between models and DTOs.
    /// This allows for easy mapping of data between different layers of the application, such as from database models to API response DTOs and from API request DTOs to database models.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.AddSpecificMappers();
        builder.AddBigMapper();
        builder.AddCreateMapper();
        return builder;
    }
    
    private static WebApplicationBuilder AddSpecificMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ChatMemberMapper>();
        builder.Services.AddTransient<MessageReadMapper>();
        builder.Services.AddTransient<ChatMapper>();
        builder.Services.AddTransient<SpaceActivityLogMapper>();

        return builder;
    }

    private static WebApplicationBuilder AddBigMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IBigMapper<Chat, ChatDto, ChatDtoShort>, ChatMapper>();
        builder.Services.AddTransient<IBigMapper<Message, MessageDto, MessageDtoShort>, MessageMapper>();
        builder.Services.AddTransient<IBigMapper<ChatMember, MemberDto, MemberDtoShort>, ChatMemberMapper>();
        builder.Services.AddTransient<IBigMapper<MessageRead, MessageReadDto, MessageReadDtoShort>, MessageReadMapper>();
        builder.Services.AddTransient<IMapper<SpaceActivityLog, SpaceActivityLogDto>, SpaceActivityLogMapper>();

        return builder;
    }
    
    private static WebApplicationBuilder AddCreateMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICreateMapper<Chat, ChatCreateDto>, ChatMapper>();
        builder.Services.AddTransient<ICreateMapper<Message, MessageCreateDto>, MessageMapper>();
        builder.Services.AddTransient<ICreateMapper<SpaceActivityLog, SpaceActivityLogCreateDto>, SpaceActivityLogMapper>();

        return builder;
    }
}
