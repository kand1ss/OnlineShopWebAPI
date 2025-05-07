using CatalogManagementService.Infrastructure;
using CatalogService.Domain;
using Core;
using RabbitMQ.Client;
using RabbitMQClient;

namespace CatalogManagementService.Application;

public class CatalogManagementServiceConfigurator(IRabbitMQClient client) 
    : IRabbitMQClientConfigurator
{
    public string ExchangeName { get; set; } = GlobalExchanges.Products;
    
    public async Task ConfigureAsync(CancellationToken ct = default)
    {
        await client.Channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, true, false, 
            cancellationToken: ct);
        
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductCreated, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductUpdated, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.ProductRemoved, true, false, 
            
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.CreateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.UpdateProduct, true, false, 
            cancellationToken: ct);
        await client.Channel.QueueDeclareAsync(GlobalQueues.RemoveProduct, true, false, 
            cancellationToken: ct);

        await client.Channel.QueueBindAsync(GlobalQueues.ProductCreated, ExchangeName, GlobalRoutingKeys.ProductCreated, 
            cancellationToken: ct);
        await client.Channel.QueueBindAsync(GlobalQueues.ProductUpdated, ExchangeName, GlobalRoutingKeys.ProductUpdated, 
            cancellationToken: ct);
        await client.Channel.QueueBindAsync(GlobalQueues.ProductRemoved, ExchangeName, GlobalRoutingKeys.ProductRemoved, 
            cancellationToken: ct);
    }
}