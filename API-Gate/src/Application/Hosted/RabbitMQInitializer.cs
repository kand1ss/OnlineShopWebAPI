using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace APIGate.Hosted;

public class RabbitMQInitializer(IConnectionService connectionService, IRabbitMQClient client) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await connectionService.ConnectWithRetriesAsync(client, cancellationToken);
        var configurator = new QueueInfrastructureInitializer(client);
        await configurator.ConfigureAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}