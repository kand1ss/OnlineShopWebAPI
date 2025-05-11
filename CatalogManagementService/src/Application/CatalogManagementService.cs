using CatalogManagementService.Application.DTO;
using CatalogService.Domain;
using Core;
using Core.Contracts;
using Core.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public class CatalogManagementService(
    IRabbitMQClient client, IConnectionService connectionService, IServiceScopeFactory scopeFactory,
    IMessageDeserializer<byte[], CreateProductRequest> createRequestDeserializer,
    IMessageDeserializer<byte[], UpdateProductRequest> updateRequestDeserializer,
    IMessageDeserializer<byte[], Guid> removeRequestDeserializer,
    ILogger<CatalogManagementService> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Initialize(stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogCritical($"Error in service: '{e.Message}'");
        }
    }

    private async Task Initialize(CancellationToken ct = default)
    {
        await connectionService.ConnectWithRetriesAsync(client, ct);
        await InitializeConsumers(ct);
    }

    private async Task InitializeConsumers(CancellationToken ct = default)
    {
        await RegisterConsumer<CreateProductRequest>(createRequestDeserializer,
            GlobalQueues.CreateProduct, GlobalRoutingKeys.ProductCreated, ct);
        await RegisterConsumer<UpdateProductRequest>(updateRequestDeserializer,
            GlobalQueues.UpdateProduct, GlobalRoutingKeys.ProductUpdated, ct);
        await RegisterConsumer<Guid>(removeRequestDeserializer,
            GlobalQueues.RemoveProduct, GlobalRoutingKeys.ProductRemoved, ct);
    }

    private async Task RegisterConsumer<TRequest>(
        IMessageDeserializer<byte[], TRequest> deserializer,
        string consumeQueue,
        string publishRoutingKey,
        CancellationToken ct = default) 
    {
        var consumer = new AsyncEventingBasicConsumer(client.Channel);

        var messageHandler = 
            new MessageHandlerConsumer<TRequest, ProductDTO>(client, scopeFactory, deserializer);
        var consumerResultPublisher =
            new ResultPublishingConsumerDecorator<ProductDTO>(client, messageHandler, publishRoutingKey, GlobalExchanges.Products);
        var consumerReplyPublisher = 
            new ReplyPublishingMessageConsumerDecorator<ProductDTO>(client, consumerResultPublisher);
        
        consumer.ReceivedAsync += consumerReplyPublisher.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(consumeQueue, false, consumer, cancellationToken: ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}