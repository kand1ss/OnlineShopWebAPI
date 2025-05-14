using CatalogManagementService.Application.DTO;
using CatalogService.Domain;
using Core;
using Core.DTO;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public class CatalogManagementService(
    IRabbitMQClient client, 
    IConnectionService connectionService,
    IConsumerRegister consumerRegister,
    MessageHandlerConsumer<ProductDTO> requestConsumer,
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
        await RegisterConsumer<CreateProductRequest>(requestConsumer, 
            GlobalQueues.CreateProduct, GlobalRoutingKeys.ProductCreated, ct);
        await RegisterConsumer<UpdateProductRequest>(requestConsumer, 
            GlobalQueues.UpdateProduct, GlobalRoutingKeys.ProductUpdated, ct);
        await RegisterConsumer<RemoveProductRequest>(requestConsumer, 
            GlobalQueues.RemoveProduct, GlobalRoutingKeys.ProductRemoved, ct);
    }

    private async Task RegisterConsumer<TRequest>(
        IMessageConsumerWithResult<ProductDTO> messageConsumer, 
        string consumeQueue,
        string publishRoutingKey,
        CancellationToken ct = default)
    {
        var consumerResultPublisher = new ResultPublishingConsumerDecorator<ProductDTO>(
            client, messageConsumer, publishRoutingKey, GlobalExchanges.Products);
        var consumerReplyPublisher = new ReplyPublishingMessageConsumerDecorator<ProductDTO>(
            client, consumerResultPublisher);
        
        await consumerRegister.RegisterConsumer<TRequest, ProductDTO>(consumerReplyPublisher, consumeQueue, ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}