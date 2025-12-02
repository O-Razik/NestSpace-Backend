using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos;
using ChatNotifyService.BLL.Dtos.Send;
using ChatNotifyService.BLL.Mappers;
using ChatNotifyService.BLL.Mappers.Send;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatNotifyService.BLL.RabbitMQ;

public class RabbitMqConsumerHostedService(
    ChatMapper chatMapper,
    RabbitMqConsumer consumer,
    ILogger<RabbitMqConsumerHostedService> logger,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting RabbitMQ consumer for ChatCreateEvent...");

        _ = consumer.StartConsuming(new RabbitMqSubscription<ChatCreateEvent>
        {
            QueueName = "chat_queue",
            ExchangeName = "chat.exchange",
            RoutingKey = "chat.create",
            HandleEvent = async evt =>
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
                            JoinedAt = evt.CreatedAt
                        }
                    }
                };
                
                await chatService.CreateChatAsync(chatMapper.ToEntity(chatDto));

                loggerScoped.LogInformation("Chat created: {ChatId}", evt.ChatId);
            }
        });
        
        _ = consumer.StartConsuming(new RabbitMqSubscription<DeleteSpaceEvent>
        {
            QueueName = "chat.space.deleted",
            ExchangeName = "space.exchange",
            RoutingKey = "space.deleted",
            HandleEvent = async evt =>
            {
                using var scope = scopeFactory.CreateScope();
                var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                logger.LogInformation("Received SpaceDeletedEvent: {SpaceId}", evt.SpaceId);

                await chatService.DeleteChatsBySpaceIdAsync(evt.SpaceId);

                logger.LogInformation("Deleted chats for space: {SpaceId}", evt.SpaceId);
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQ consumer...");
        _ = consumer.Stop();
        return Task.CompletedTask;
    }
}