namespace ChatNotifyService.ABS.Helpers;

public interface IEventConsumer
{
    Task ConsumeAsync<TEvent>(
        Func<TEvent, Task> handleEvent,
        string queueName,
        string routingKey,
        string exchangeName)
        where TEvent : class;
}
