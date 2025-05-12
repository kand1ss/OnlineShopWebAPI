using CatalogService.Domain;
using Core;
using RabbitMQ.Client;

namespace RabbitMQClient;

public class QueueInfrastructureInitializer(IRabbitMQClient client) : IRabbitMQClientConfigurator
{
    public async Task ConfigureAsync(CancellationToken ct = default)
    {
        await client.Channel.ExchangeDeclareAsync(GlobalExchanges.Products, ExchangeType.Topic, true, false, 
            cancellationToken: ct);
        
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductCreated, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductUpdated, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductRemoved, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductOperationReply, true, false, 
            autoDelete: false, cancellationToken: ct);
        
        await client.Channel.QueueBindAsync(GlobalQueues.ProductCreated, GlobalExchanges.Products, 
            GlobalRoutingKeys.ProductCreated, cancellationToken: ct);
        await client.Channel.QueueBindAsync(GlobalQueues.ProductUpdated, GlobalExchanges.Products, 
            GlobalRoutingKeys.ProductUpdated, cancellationToken: ct);
        await client.Channel.QueueBindAsync(GlobalQueues.ProductRemoved, GlobalExchanges.Products, 
            GlobalRoutingKeys.ProductRemoved, cancellationToken: ct);
        
        await client.Channel.QueueDeclareAsync(GlobalQueues.CreateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.UpdateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.RemoveProduct, true, false, 
            cancellationToken: ct);
    }
}