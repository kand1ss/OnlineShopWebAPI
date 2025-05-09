using API_Gate;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using RabbitMQ.Client.Events;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace APIGate.Consumers;

public class ProductReplyConsumer(
    IRabbitMQClient client,
    IMessageDeserializer<byte[], ProductOperationReply> deserializer,
    string expectedId,
    TaskCompletionSource<ProductReply> tcs) : IMessageConsumer
{
    public async Task ProcessConsumeAsync(object model, BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.CorrelationId != expectedId)
            return;
        
        var data = deserializer.Deserialize(ea.Body.ToArray());

        tcs.SetResult(new ProductReply { Message = data.Message, Success = data.Success });
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
    }
}