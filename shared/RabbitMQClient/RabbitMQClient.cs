using RabbitMQ.Client;

namespace RabbitMQClient;

public class RabbitMQClient : IRabbitMQClient, IDisposable, IAsyncDisposable
{
    private readonly string _hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
    
    private IChannel? _channel;
    private IConnection? _connection;
    public IChannel Channel => _channel ?? throw new Exception("Channel is not initialized");
    public IConnection Connection => _connection ?? throw new Exception("Connection is not initialized");
    
    public async Task CreateConnectionAsync(CancellationToken ct = default)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _hostName
        };

        _connection = await connectionFactory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        var configurator = new QueueInfrastructureInitializer(this);
        await configurator.ConfigureAsync(ct);
    }

    public async Task CloseConnectionAsync(CancellationToken ct = default)
    {
        if (_channel != null) 
            await _channel.CloseAsync(ct);
        if (_connection != null) 
            await _connection.CloseAsync(ct);
    }

    
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) 
            await _channel.DisposeAsync();
        if (_connection != null) 
            await _connection.DisposeAsync();
    }
}