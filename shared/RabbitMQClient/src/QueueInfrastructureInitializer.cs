using Core;
using RabbitMQ.Client;

namespace RabbitMQClient;

public class QueueInfrastructureInitializer(IRabbitMQClient client) : IRabbitMQClientConfigurator
{
    public async Task ConfigureAsync(CancellationToken ct = default)
    {
        await client.Channel.ExchangeDeclareAsync(GlobalExchanges.Products, ExchangeType.Topic, 
            true, false, cancellationToken: ct);
        await client.Channel.ExchangeDeclareAsync(GlobalExchanges.Accounts, ExchangeType.Topic, 
            true, false, cancellationToken: ct);


        await client.Channel.QueueDeclareAsync(GlobalQueues.CreateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.UpdateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.RemoveProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductOperationReply, true, false, 
            autoDelete: false, cancellationToken: ct);

        
        await client.Channel.QueueDeclareAsync(GlobalQueues.CreateAccount, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.UpdateAccount, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.RemoveAccount, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.AccountOperationReply, true, false, 
            false, cancellationToken: ct);
    }
}