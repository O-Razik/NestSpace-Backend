using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos.Send;
using ChatNotifyService.BLL.Mappers.Send;
using ChatNotifyService.BLL.RabbitMQ.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatNotifyService.BLL.RabbitMQ.Consumer;

public class RabbitMqConsumerHostedService(
    ChatMapper chatMapper,
    RabbitMqConsumer consumer,
    ILogger<RabbitMqConsumerHostedService> logger,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting RabbitMQ consumer for ChatCreateEvent...");

        await consumer.StartConsuming(new RabbitMqSubscription<ChatCreateEvent>
        {
            QueueName = "chat.queue",
            ExchangeName = "chat.exchange",
            RoutingKey = "chat.create",
            HandleEvent = async evt =>
            {
                await HandleSpaceCreation(evt);
            }
        });
        
        await consumer.StartConsuming(new RabbitMqSubscription<DeleteSpaceEvent>
        {
            QueueName = "chat.space.deleted",
            ExchangeName = "space.exchange",
            RoutingKey = "space.deleted",
            HandleEvent = async evt =>
            {
                await HandleSpaceDeletion(evt);
            }
        });
        
        await consumer.StartConsuming(new RabbitMqSubscription<SpaceActivityLogEvent>
        {
            QueueName = "chat.space.activity.log",
            ExchangeName = "log.exchange",
            RoutingKey = "space.activity.log",
            HandleEvent = async evt =>
            {
                await HandleActivityLog(evt);
            }
        });
    }

    private async Task HandleSpaceCreation(ChatCreateEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        var loggerScoped = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqConsumerHostedService>>();
                
        loggerScoped.LogInformation("Received ChatCreateEvent for SpaceId {SpaceId}", evt.SpaceId);

        var chatDto = new ChatDto()
        {
            Id = evt.ChatId,
            SpaceId = evt.SpaceId,
            Name = "Main Chat",
            Members = new List<MemberDto>()
            {
                new MemberDto
                {
                    MemberId = evt.MemberId,
                    ChatId = evt.ChatId,
                    PermissionLevel = PermissionLevel.Admin,
                    JoinedAt = evt.CreatedAt
                }
            }
        };
                
        await chatService.CreateChatAsync(chatMapper.ToEntity(chatDto));

        loggerScoped.LogInformation("Chat created: {ChatId}", evt.ChatId);
    }
    
    private async Task HandleSpaceDeletion(DeleteSpaceEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        var loggerScoped = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqConsumerHostedService>>();
                
        loggerScoped.LogInformation("Received DeleteSpaceEvent for SpaceId {SpaceId}", evt.SpaceId);

        await chatService.DeleteChatsBySpaceIdAsync(evt.SpaceId);

        loggerScoped.LogInformation("Deleted chats for SpaceId {SpaceId}", evt.SpaceId);
    }
    
    private async Task<SpaceActivityLogEvent> HandleActivityLog(SpaceActivityLogEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var activityLogService = scope.ServiceProvider.GetRequiredService<ISpaceActivityLogService>();
        var logMapper = scope.ServiceProvider.GetRequiredService<SpaceActivityLogMapper>();

        logger.LogInformation("Received SpaceActivityLogEvent: {SpaceId}, Type: {Type}, Description: {Description}",
            evt.SpaceId, evt.Type, evt.Description);

        var activityLog = new SpaceActivityLogDto()
        {
            SpaceId = evt.SpaceId,
            Type = evt.Type,
            Description = evt.Description,
            CreatedAt = evt.ActivityAt
        };

        await activityLogService.CreateActivityLogAsync(logMapper.ToEntity(activityLog));

        logger.LogInformation("Logged activity for space: {SpaceId}, Type: {Type}, Description: {Description}",
            evt.SpaceId, evt.Type, evt.Description);

        return evt;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQ consumer...");
        _ = consumer.Stop();
        return Task.CompletedTask;
    }
}