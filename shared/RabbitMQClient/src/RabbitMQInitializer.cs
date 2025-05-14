using Microsoft.Extensions.Hosting;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace CatalogManagementGateway.Hosted;

public class RabbitMQInitializer(IConnectionService connectionService, IRabbitMQClient client) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await connectionService.ConnectWithRetriesAsync(client, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}