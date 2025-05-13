using CatalogManagementService.Application.Replies;
using Core.Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace CatalogManagementGateway.Application;

/// <summary>
/// Represents a client for publishing messages to a RabbitMQ broker and consuming replies asynchronously.
/// This class supports workflows where a message is published to a specific routing key,
/// and a correlated reply is awaited from a specified reply queue.
/// </summary>
/// <typeparam name="TReplyResult">The type of the reply result expected to be deserialized from the consumed message.</typeparam>
public class MessageRequestClient<TReplyResult>(
    IRabbitMQClient client,
    IMessageDeserializer<byte[], RequestReply<TReplyResult>> replyDeserializer,
    ILogger<MessageRequestClient<TReplyResult>> logger
    ) : IMessageRequestClient<TReplyResult>
{
    private readonly TimeSpan _defaultReplyTimeout = TimeSpan.FromSeconds(15);
    
    
    /// Publishes a message to the specified routing key and waits for a corresponding reply message from the reply queue.
    /// <param name="body">The message body to be published.</param>
    /// <param name="publishRoutingKey">The routing key used to publish the message.</param>
    /// <param name="replyQueue">The name of the reply queue where the response message will be consumed.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the reply message deserialized into a <see cref="RequestReply{TReplyResult}"/>.</returns>
    public async Task<RequestReply<TReplyResult>> PublishMessageAndConsumeReply(byte[] body, string publishRoutingKey, string replyQueue, TimeSpan? timeout = null)
    {
        timeout ??= _defaultReplyTimeout;
        
        var generatedId = Guid.NewGuid().ToString();
        try
        {
            return await TryPublishMessageAndConsumeReply(body, generatedId, publishRoutingKey, replyQueue);
        }
        catch (Exception e)
        {
            logger.LogError($"[{generatedId}] Error publishing message to queue '{publishRoutingKey}': '{e.Message}'");
            return RequestReply<TReplyResult>.Fail(e.Message);
        }
    }

    private async Task<RequestReply<TReplyResult>> TryPublishMessageAndConsumeReply(byte[] body, string generatedId, string publishRoutingKey, string replyQueue)
    {
        var props = CreateBasicProperties(generatedId, replyQueue);
        
        logger.LogInformation(
            $"[{generatedId}] Requested to publish a message to queue '{publishRoutingKey}' with the response returned in queue '{replyQueue}'");

        await client.Channel.BasicPublishAsync("", publishRoutingKey, true, props, body);
        
        logger.LogInformation($"[{generatedId}] Message published to queue '{publishRoutingKey}'");
        
        var tcs = new TaskCompletionSource<RequestReply<TReplyResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        var replyConsumer = new ReplyConsumer<TReplyResult>(client, replyDeserializer, generatedId, tcs);
        
        consumer.ReceivedAsync += replyConsumer.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(replyQueue, false, consumer);
        
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(_defaultReplyTimeout));
        if (completedTask == tcs.Task)
        {
            if (consumer.ConsumerTags.Length > 0)
                await client.Channel.BasicCancelAsync(consumer.ConsumerTags.First());
            
            logger.LogInformation($"[{generatedId}] Reply received from queue '{replyQueue}' within the defined timeout period.");
            return tcs.Task.Result;
        }

        logger.LogWarning($"[{generatedId}] No reply received from queue '{replyQueue}' within the defined timeout period.");
        return RequestReply<TReplyResult>.Fail("No response was received within the defined timeout period.");
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