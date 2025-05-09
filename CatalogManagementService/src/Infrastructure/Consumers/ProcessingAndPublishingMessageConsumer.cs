using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.Replies;
using Core;
using Core.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application.Consumers;

public class ProcessingAndPublishingMessageConsumer<TRequest>(
    IRabbitMQClient client, 
    IServiceScopeFactory scopeFactory,
    IMessageDeserializer<byte[], TRequest> deserializer,
    string publishRoutingKey) : IMessageConsumer
{
    public async Task ProcessConsumeAsync(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            await TryProcessConsumeAsync(ea);
        }
        catch (Exception e)
        {
            await SendReply(ea.BasicProperties, new ProductOperationReply(e.Message, false));
            throw;
        }
    }

    private async Task TryProcessConsumeAsync(BasicDeliverEventArgs ea)
    {
        var data = deserializer.Deserialize(ea.Body.ToArray());
        
        using var scope = scopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<IRequestProcessor<TRequest, Product>>();
        var product = await processor.Process(data);
        
        await OnProcessed(product);
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
        await SendReply(ea.BasicProperties, new ProductOperationReply("Product successfully created", true));
    }

    private async Task OnProcessed(Product product)
    {
        var json = JsonSerializer.Serialize(product.ToDTO());
        var body = Encoding.UTF8.GetBytes(json);
        
        await client.Channel.BasicPublishAsync(GlobalExchanges.Products, publishRoutingKey, 
            true, body: body);
    }

    private async Task SendReply(IReadOnlyBasicProperties requestProperties, ProductOperationReply reply)
    {
        var props = new BasicProperties
        {
            CorrelationId = requestProperties.CorrelationId,
            ReplyTo = requestProperties.ReplyTo,
        };
        
        var json = JsonSerializer.Serialize(reply);
        var body = Encoding.UTF8.GetBytes(json);

        if (props.ReplyTo != null)
            await client.Channel.BasicPublishAsync(
                "", props.ReplyTo, true, props, body);
    }
}