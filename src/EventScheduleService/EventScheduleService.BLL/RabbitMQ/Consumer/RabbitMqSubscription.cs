namespace EventScheduleService.BLL.RabbitMQ.Consumer;

public class RabbitMqSubscription<TEvent> where TEvent : class
{
    public required  string QueueName { get; init; }
    public required  string ExchangeName { get; init; }
    public required  string RoutingKey { get; init; }
    public required Func<TEvent, Task> HandleEvent { get; init; }
}
