namespace UserSpaceService.ABS.IHelpers;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent createdEvent, string routingKey, string exchangeName)
        where TEvent : class;
}