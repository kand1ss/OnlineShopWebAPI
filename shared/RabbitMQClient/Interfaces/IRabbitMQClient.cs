using RabbitMQ.Client;

namespace RabbitMQClient;

public interface IRabbitMQClient
{
    public IChannel Channel { get; }
    public IConnection Connection { get; }
    
    Task CreateConnectionAsync(CancellationToken ct = default);
    Task CloseConnectionAsync(CancellationToken ct = default);
}