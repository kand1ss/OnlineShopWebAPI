using CatalogManagementService.Application.Consumers;
using CatalogManagementService.Application.DTO;
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
        var createRequestDeserializer = new CreateProductRequestDeserializer();
        var updateRequestDeserializer = new UpdateProductRequestDeserializer();
        var removeRequestDeserializer = new RemoveProductRequestDeserializer();

        await RegisterConsumer<CreateProductRequest>(createRequestDeserializer,
            GlobalQueues.CreateProduct, GlobalRoutingKeys.ProductCreated, ct);
        await RegisterConsumer<UpdateProductRequest>(updateRequestDeserializer,
            GlobalQueues.UpdateProduct, GlobalRoutingKeys.ProductUpdated, ct);
        await RegisterConsumer<Guid>(removeRequestDeserializer,
            GlobalQueues.RemoveProduct, GlobalRoutingKeys.ProductRemoved, ct);
    }

    private async Task RegisterConsumer<TRequest>(
        IMessageDeserializer<string, TRequest> deserializer,
        string consumeQueue,
        string publishRoutingKey,
        CancellationToken ct = default) 
    {
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        var messageConsumer = new ProcessingAndPublishingMessageConsumer<TRequest>(client, scopeFactory, deserializer, 
            publishRoutingKey);
        
        consumer.ReceivedAsync += messageConsumer.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(consumeQueue, false, consumer, cancellationToken: ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}