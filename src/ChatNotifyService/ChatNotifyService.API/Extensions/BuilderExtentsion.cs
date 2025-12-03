using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos.Create;
using ChatNotifyService.BLL.Dtos.Send;
using ChatNotifyService.BLL.Mappers.Create;
using ChatNotifyService.BLL.Mappers.Send;
using ChatNotifyService.BLL.RabbitMQ;
using ChatNotifyService.BLL.RabbitMQ.Consumer;
using ChatNotifyService.BLL.Services;
using ChatNotifyService.DAL.Entities;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Factories;
using ChatNotifyService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.API.Extensions;

public static class BuilderExtension
{
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
    
    public static WebApplicationBuilder AddRabbitMqServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
        builder.Configuration.GetSection("RabbitSettings"));
         
        builder.Services.AddSingleton<RabbitMqConsumer>();
        builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
        
        return builder;
    }

    public static WebApplicationBuilder AddEntities(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChat, Chat>();
        builder.Services.AddScoped<IMessage, Message>();
        builder.Services.AddScoped<IChatMember, ChatMember>();
        builder.Services.AddScoped<IMessageRead, MessageRead>();
        
        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IMessageReadRepository, MessageReadRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IChatNotificationService, ChatNotificationService>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEntityFactory<IChat>, ChatFactory>();
        builder.Services.AddTransient<IEntityFactory<IMessage>, MessageFactory>();
        builder.Services.AddTransient<IEntityFactory<IChatMember>, ChatMemberFactory>();
        builder.Services.AddTransient<IEntityFactory<IMessageRead>, MessageReadFactory>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ChatMemberMapper>();
        builder.Services.AddTransient<MessageReadMapper>();
        builder.Services.AddTransient<ChatMapper>();
        
        
        builder.Services.AddTransient<IBigMapper<IChat, ChatDto, ChatDtoShort>, ChatMapper>();
        builder.Services.AddTransient<IBigMapper<IMessage, MessageDto, MessageDtoShort>, MessageMapper>();
        builder.Services.AddTransient<IBigMapper<IChatMember, MemberDto, MemberDtoShort>, ChatMemberMapper>();
        builder.Services.AddTransient<IBigMapper<IMessageRead, MessageReadDto, MessageReadDtoShort>, MessageReadMapper>();
        
        builder.Services.AddTransient<ICreateMapper<IChat, ChatCreateDto>, ChatCreateMapper>();
        builder.Services.AddTransient<ICreateMapper<IMessage, MessageCreateDto>, MessageCreateMapper>();
        builder.Services.AddTransient<ICreateMapper<IChatMember, MemberCreateDto>, ChatMemberCreateMapper>();
        
        return builder;
    }
}