using Microsoft.Extensions.Logging;
using RabbitMQClient.Contracts;

namespace RabbitMQClient;

public class RabbitMQConnectionService(ILogger<RabbitMQConnectionService> logger) : IConnectionService
{
    public async Task ConnectWithRetriesAsync(IRabbitMQClient client, CancellationToken ct)
    {
        var delay = TimeSpan.FromSeconds(1);
        const int maxDelaySeconds = 30;

        while (true)
        {
            try
            {
                await client.CreateConnectionAsync(ct);
                logger.LogInformation("Connected to RabbitMQ");
                return;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("RabbitMQ connection attempt cancelled.");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error connecting to RabbitMQ: '{ex.Message}'; Retrying in {delay.TotalSeconds}s");
                try
                {
                    await Task.Delay(delay, ct);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Connection to RabbitMQ cancelled.");
                }
                
                delay = TimeSpan.FromSeconds(Math.Min(maxDelaySeconds, delay.TotalSeconds * 2));
            }
        }
    }
}