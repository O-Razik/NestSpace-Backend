using System.Text;
using System.Text.Json;
using EventScheduleService.ABS.IHelpers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EventScheduleService.BLL.RabbitMQ.Publisher;

public class RabbitMqPublisher(IOptions<RabbitSettings> rabbitSettings) : IEventPublisher
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishAsync<TEvent>(TEvent createdEvent, string routingKey, string exchangeName) where TEvent : class
    {
        await SetupAsync();

        if (_channel is null)
        {
            throw new InvalidOperationException("Channel not initialized");
        }

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(createdEvent));

        await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        await _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            body: body
        );

        await Task.CompletedTask;
    }

    private async Task SetupAsync()
    {
        if (_channel is not null)
        {
            return;
        }

        var factory = new ConnectionFactory()
        {
            HostName = rabbitSettings.Value.HostName,
            Port = rabbitSettings.Value.Port,
            UserName = rabbitSettings.Value.UserName,
            Password = rabbitSettings.Value.Password
        };

        _connection = await factory.CreateConnectionAsync(); 
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.ExchangeDeclareAsync(exchange: "event.exchange", type: ExchangeType.Topic, durable: true);
        await _channel.ExchangeDeclareAsync(exchange: "log.exchange", type: ExchangeType.Topic, durable: true);
    }
}