namespace RabbitMQClient.Contracts;

public interface IConnectionService
{
    Task ConnectWithRetriesAsync(IRabbitMQClient client, CancellationToken ct);
}