using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventScheduleService.BLL.RabbitMQ.Consumer;

public class RabbitMqConsumer(IOptions<RabbitSettings> options) : IDisposable, IAsyncDisposable
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
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<TEvent>(body);
                if (message != null)
                {
                    await subscription.HandleEvent(message);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (JsonException)
            {
                // JSON deserialization failed, reject and don't requeue
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
            }
            catch (Exception) when (ea.DeliveryTag > 0)
            {
                // Processing failed, reject and requeue for retry
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(subscription.QueueName, autoAck: false, consumer: consumer);
    }

    public async Task StopAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_channel != null)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
        }
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeCoreAsync().ConfigureAwait(false);
        
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed resources
            _channel?.Dispose();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    protected virtual async ValueTask DisposeCoreAsync()
    {
        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync().ConfigureAwait(false);
            }
            catch (AlreadyClosedException)
            {
                // Channel already closed, ignore
            }
            finally
            {
                _channel.Dispose();
            }
        }

        if (_connection != null)
        {
            try
            {
                await _connection.CloseAsync().ConfigureAwait(false);
            }
            catch (AlreadyClosedException)
            {
                // Connection already closed, ignore
            }
            finally
            {
                _connection.Dispose();
            }
        }
    }
}
