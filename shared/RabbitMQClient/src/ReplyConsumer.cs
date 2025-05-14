using CatalogManagementService.Application.Replies;
using Core.Contracts;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

/// <summary>
/// Handles the consumption of messages from a RabbitMQ queue for request-reply messaging.
/// </summary>
/// <typeparam name="TReplyResult">The type of the reply message being processed.</typeparam>
public class ReplyConsumer<TReplyResult>(
    IRabbitMQClient client,
    IRequestDeserializer deserializer,
    string expectedId,
    TaskCompletionSource<RequestReply<TReplyResult>> tcs) : IMessageConsumer
{
    public async Task ProcessConsumeAsync<TRequest>(object model, BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.CorrelationId != expectedId)
            return;
        
        var data = deserializer.Deserialize<RequestReply<TReplyResult>>(ea.Body.ToArray());
        tcs.SetResult(data);
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
    }
}