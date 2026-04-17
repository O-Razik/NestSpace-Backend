using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventScheduleService.BLL.RabbitMQ.Consumer;

public class RabbitMqConsumerHostedService(
    RabbitMqConsumer consumer,
    ILogger<RabbitMqConsumerHostedService> logger,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = consumer.StartConsumingAsync(new RabbitMqSubscription<DeleteSpaceEvent>
        {
            QueueName = "event.space.deleted",
            ExchangeName = "space.exchange",
            RoutingKey = "space.deleted",
            HandleEvent = async evt =>
            {
                using var scope = scopeFactory.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                var soloEventService = scope.ServiceProvider.GetRequiredService<ISoloEventService>();
                var regularEventService = scope.ServiceProvider.GetRequiredService<IRegularEventService>();

                logger.LogInformation("Received SpaceDeletedEvent: {SpaceId}", evt.SpaceId);

                await soloEventService.DeleteSoloEventsBySpaceIdAsync(evt.SpaceId);
                logger.LogInformation("Deleted solo events for space: {SpaceId}", evt.SpaceId);
                
                await regularEventService.DeleteRegularEventsBySpaceIdAsync(evt.SpaceId);
                logger.LogInformation("Deleted regular events for space: {SpaceId}", evt.SpaceId);
                
                await eventService.DeleteCategoryBySpaceIdAsync(evt.SpaceId);
                logger.LogInformation("Deleted categories for space: {SpaceId}", evt.SpaceId);
                
                await eventService.DeleteTagBySpaceIdAsync(evt.SpaceId);
                logger.LogInformation("Deleted tags for space: {SpaceId}", evt.SpaceId);
            }
        });
        
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQ consumer...");
        _ = consumer.StopAsync();
        return Task.CompletedTask;
    }
}