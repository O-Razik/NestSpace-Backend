using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Client = RabbitMQ.Client;

namespace ChatNotifyService.BLL.RabbitMQ.Consumer;

public sealed class RabbitMqConsumer(IOptions<RabbitSettings> options) : IDisposable, IAsyncDisposable
{
    private readonly RabbitSettings _settings = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    public async Task StartConsumingAsync<TEvent>(RabbitMqSubscription<TEvent> subscription) where TEvent : class
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
        consumer.ReceivedAsync += (_, ea) => HandleReceivedAsync(subscription, ea);

        await _channel.BasicConsumeAsync(subscription.QueueName, autoAck: false, consumer: consumer);
    }

    private async Task HandleReceivedAsync<TEvent>(RabbitMqSubscription<TEvent> subscription, BasicDeliverEventArgs ea) where TEvent : class
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<TEvent>(body);
            if (message != null)
            {
                await subscription.HandleEvent(message);
            }
            
            if (_channel != null)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        }
        catch (JsonException)
        {
            if (_channel != null)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            }
        }
        catch (InvalidOperationException)
        {
            if (_channel != null)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        }
        catch (OperationCanceledException)
        {
            if (_channel != null)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        }
        catch (Exception ex) when (
            ex is Client.Exceptions.BrokerUnreachableException or 
                Client.Exceptions.AlreadyClosedException)
        {
            if (_channel != null)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        }
        // Optionally log unexpected exceptions here
    }

    public async Task StopAsync()
    {
        EnsureNotDisposed();

        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMqConsumer));
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _channel = null;
            _connection = null;
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
            _channel = null;
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
            _connection = null;
        }

        Dispose(disposing: true);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
