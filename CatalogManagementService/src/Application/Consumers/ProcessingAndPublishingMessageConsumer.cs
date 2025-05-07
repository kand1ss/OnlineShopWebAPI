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

public class ProcessingAndPublishingMessageConsumer<TProcessor>(
    IRabbitMQClient client, 
    IServiceScopeFactory scopeFactory,
    IMessageDeserializer<string, ProductDTO> deserializer,
    string publishRoutingKey) : IMessageConsumer where TProcessor : IRequestProcessor<ProductDTO, Product>
{
    public async Task ProcessConsumeAsync(object model, BasicDeliverEventArgs ea)
    {
        var data = deserializer.Deserialize(ea.GetBodyAsString());
        
        using var scope = scopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<TProcessor>();
        var product = await processor.Process(data);
        
        await OnProcessed(product);
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
    }
    
    private async Task OnProcessed(Product product)
    {
        var json = JsonSerializer.Serialize(product);
        var body = Encoding.UTF8.GetBytes(json);
        
        await client.Channel.BasicPublishAsync(GlobalExchanges.Products, publishRoutingKey, 
            true, body: body);
    }
}