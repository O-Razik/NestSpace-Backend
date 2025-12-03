namespace EventScheduleService.BLL.RabbitMQ.Consumer;

public class RabbitMqSubscription<TEvent> where TEvent : class
{
    public string QueueName { get; set; } = null!;
    public string ExchangeName { get; set; } = null!;
    public string RoutingKey { get; set; } = null!;
    public Func<TEvent, Task> HandleEvent { get; set; } = null!;
}