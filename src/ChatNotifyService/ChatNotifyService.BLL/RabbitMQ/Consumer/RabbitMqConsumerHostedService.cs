using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Mappers;
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

        await consumer.StartConsumingAsync(new RabbitMqSubscription<ChatCreateEvent>
        {
            QueueName = "chat.queue",
            ExchangeName = "chat.exchange",
            RoutingKey = "chat.create",
            HandleEvent = async evt =>
            {
                await HandleSpaceCreationAsync(evt);
            }
        });
        
        await consumer.StartConsumingAsync(new RabbitMqSubscription<DeleteSpaceEvent>
        {
            QueueName = "chat.space.deleted",
            ExchangeName = "space.exchange",
            RoutingKey = "space.deleted",
            HandleEvent = async evt =>
            {
                await HandleSpaceDeletionAsync(evt);
            }
        });
        
        await consumer.StartConsumingAsync(new RabbitMqSubscription<SpaceActivityLogEvent>
        {
            QueueName = "chat.space.activity.log",
            ExchangeName = "log.exchange",
            RoutingKey = "space.activity.log",
            HandleEvent = async evt =>
            {
                await HandleActivityLogAsync(evt);
            }
        });
    }

    private async Task HandleSpaceCreationAsync(ChatCreateEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        var loggerScoped = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqConsumerHostedService>>();
                
        loggerScoped.LogInformation("Received ChatCreateEvent for SpaceId {SpaceId}", evt.SpaceId);

        var chatDto = new ChatCreateDto
        {
            SpaceId = evt.SpaceId,
            Name = "Main Chat",
            Members = new List<MemberCreateDto>
            {
                new MemberCreateDto
                {
                    MemberId = evt.MemberId,
                    PermissionLevel = PermissionLevel.Admin,
                }
            }
        };
                
        await chatService.CreateChatAsync(chatDto);

        loggerScoped.LogInformation("Chat created: {ChatId}", evt.ChatId);
    }
    
    private async Task HandleSpaceDeletionAsync(DeleteSpaceEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        var loggerScoped = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqConsumerHostedService>>();
                
        loggerScoped.LogInformation("Received DeleteSpaceEvent for SpaceId {SpaceId}", evt.SpaceId);

        await chatService.DeleteChatsBySpaceIdAsync(evt.SpaceId);

        loggerScoped.LogInformation("Deleted chats for SpaceId {SpaceId}", evt.SpaceId);
    }
    
    private async Task<SpaceActivityLogEvent> HandleActivityLogAsync(SpaceActivityLogEvent evt)
    {
        using var scope = scopeFactory.CreateScope();
        var activityLogService = scope.ServiceProvider.GetRequiredService<ISpaceActivityLogService>();

        logger.LogInformation("Received SpaceActivityLogEvent: {SpaceId}, Type: {Type}, Description: {Description}",
            evt.SpaceId, evt.Type, evt.Description);

        var activityLog = new SpaceActivityLogCreateDto
        {
            SpaceId = evt.SpaceId,
            Type = evt.Type,
            Description = evt.Description
        };

        await activityLogService.CreateActivityLogAsync(activityLog);

        logger.LogInformation("Logged activity for space: {SpaceId}, Type: {Type}, Description: {Description}",
            evt.SpaceId, evt.Type, evt.Description);

        return evt;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQ consumer...");
        _ = consumer.StopAsync();
        return Task.CompletedTask;
    }
}
