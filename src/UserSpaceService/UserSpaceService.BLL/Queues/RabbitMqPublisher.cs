using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserSpaceService.ABS.IHelpers;

namespace UserSpaceService.BLL.Queues;

public class RabbitMqPublisher(
    IOptions<RabbitSettings> rabbitSettings,
    ILogger<RabbitMqPublisher> logger) : IEventPublisher
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
        
        logger.LogInformation(
            "Publishing event to Exchange: {Exchange}, RoutingKey: {RoutingKey}", 
            exchangeName, 
            routingKey
        );

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
        
        await _channel.ExchangeDeclareAsync(exchange: "chat.exchange", type: ExchangeType.Topic, durable: true);
        await _channel.ExchangeDeclareAsync(exchange: "space.exchange", type: ExchangeType.Topic, durable: true);
        await _channel.ExchangeDeclareAsync(exchange: "log.exchange", type: ExchangeType.Topic, durable: true);

    }
}