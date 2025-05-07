using CatalogManagementService.Application.Consumers;
using CatalogService.Domain;
using Core;
using Core.Contracts;
using Core.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;

namespace CatalogManagementService.Application;

public class CatalogManagementService(IRabbitMQClient client, IServiceScopeFactory scopeFactory) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await client.CreateConnectionAsync(stoppingToken);
        var configurator = new CatalogManagementServiceConfigurator(client);

        await configurator.ConfigureAsync(stoppingToken);
        await InitializeConsumers(stoppingToken);
    }

    private async Task InitializeConsumers(CancellationToken ct = default)
    {
        var deserializer = new ProductDTODeserializer();

        await RegisterConsumer<CreateProductRequestProcessor>(deserializer,
            GlobalQueues.CreateProduct, GlobalRoutingKeys.ProductCreated, ct);
        await RegisterConsumer<UpdateProductRequestProcessor>(deserializer,
            GlobalQueues.UpdateProduct, GlobalRoutingKeys.ProductUpdated, ct);
        await RegisterConsumer<RemoveProductRequestProcessor>(deserializer,
            GlobalQueues.RemoveProduct, GlobalRoutingKeys.ProductRemoved, ct);
    }

    private async Task RegisterConsumer<TProcessor>(
        IMessageDeserializer<string, ProductDTO> deserializer,
        string consumeQueue,
        string publishRoutingKey,
        CancellationToken ct = default) 
        where TProcessor : IRequestProcessor<ProductDTO, Product>
    {
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        var messageConsumer = new ProcessingAndPublishingMessageConsumer<TProcessor>(client, scopeFactory, deserializer, 
            publishRoutingKey);
        
        consumer.ReceivedAsync += messageConsumer.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(consumeQueue, false, consumer, cancellationToken: ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}