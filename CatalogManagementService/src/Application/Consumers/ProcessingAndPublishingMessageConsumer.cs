using System.Text;
using System.Text.Json;
using Core;
using Core.Contracts;
using Core.DTO;
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
        var data = deserializer.Deserialize(ea.Body.ToArray());
        
        using var scope = scopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<IRequestProcessor<TRequest, Product>>();
        var product = await processor.Process(data);
        
        await OnProcessed(product);
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
    }
    
    private async Task OnProcessed(Product product)
    {
        var json = JsonSerializer.Serialize(product.ToDTO());
        var body = Encoding.UTF8.GetBytes(json);
        
        await client.Channel.BasicPublishAsync(GlobalExchanges.Products, publishRoutingKey, 
            true, body: body);
    }
}