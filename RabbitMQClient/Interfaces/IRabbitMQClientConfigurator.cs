namespace RabbitMQClient;

public interface IRabbitMQClientConfigurator
{
    Task ConfigureAsync(CancellationToken ct = default);
}