using APIGate.Consumers;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace APIGate.Application;

/// <summary>
/// Represents a client for publishing messages to a RabbitMQ broker and consuming replies asynchronously.
/// This class supports workflows where a message is published to a specific routing key,
/// and a correlated reply is awaited from a specified reply queue.
/// </summary>
/// <typeparam name="TReplyResult">The type of the reply result expected to be deserialized from the consumed message.</typeparam>
public class MessageRequestClient<TReplyResult>(
    IRabbitMQClient client,
    IMessageDeserializer<byte[], RequestReply<TReplyResult>> replyDeserializer
    ) : IMessageRequestClient<TReplyResult>
{
    /// Publishes a message to the specified routing key and waits for a corresponding reply message from the reply queue.
    /// <param name="body">The message body to be published.</param>
    /// <param name="publishRoutingKey">The routing key used to publish the message.</param>
    /// <param name="replyQueue">The name of the reply queue where the response message will be consumed.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the reply message deserialized into a <see cref="RequestReply{TReplyResult}"/>.</returns>
    /// <exception cref="TimeoutException">Thrown if no reply is received within the defined timeout period.</exception>
    public async Task<RequestReply<TReplyResult>> PublishMessageAndConsumeReply(byte[] body, string publishRoutingKey, string replyQueue)
    {
        var generatedId = Guid.NewGuid().ToString();
        var props = CreateBasicProperties(generatedId, replyQueue);

        await client.Channel.BasicPublishAsync("", publishRoutingKey, true, props, body);
        
        var tcs = new TaskCompletionSource<RequestReply<TReplyResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        var replyConsumer = new ReplyConsumer<TReplyResult>(client, replyDeserializer, generatedId, tcs);
        
        consumer.ReceivedAsync += replyConsumer.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(replyQueue, false, consumer);
        
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));
        if (completedTask == tcs.Task)
        {
            if (consumer.ConsumerTags.Length > 0)
                await client.Channel.BasicCancelAsync(consumer.ConsumerTags.First());
            return tcs.Task.Result;
        }

        throw new TimeoutException("Response timeout exceeded");
    }
    
    private BasicProperties CreateBasicProperties(string generatedId, string replyQueue)
    {
        return new BasicProperties
        {
            CorrelationId = generatedId,
            ReplyTo = replyQueue
        };
    }
}