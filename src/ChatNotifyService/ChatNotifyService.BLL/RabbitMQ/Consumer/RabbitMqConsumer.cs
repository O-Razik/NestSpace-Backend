using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatNotifyService.BLL.RabbitMQ.Consumer;

public class RabbitMqConsumer(IOptions<RabbitSettings> options) : IDisposable, IAsyncDisposable
{
    private readonly RabbitSettings _settings = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    public async Task StartConsuming<TEvent>(RabbitMqSubscription<TEvent> subscription) where TEvent : class
    {
        EnsureNotDisposed();

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(subscription.ExchangeName, ExchangeType.Topic, durable: true);
        await _channel.QueueDeclareAsync(subscription.QueueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(subscription.QueueName, subscription.ExchangeName, subscription.RoutingKey);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<TEvent>(body);
                if (message != null)
                    await subscription.HandleEvent(message);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(subscription.QueueName, autoAck: false, consumer: consumer);
    }

    public async Task Stop()
    {
        if (_disposed) return;

        await _channel?.CloseAsync()!;
        await _connection?.CloseAsync()!;
    }

    private void EnsureNotDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(RabbitMqConsumer));
    }

    public void Dispose()
    {
        if (_disposed) return;

        _channel?.Dispose();
        _connection?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}