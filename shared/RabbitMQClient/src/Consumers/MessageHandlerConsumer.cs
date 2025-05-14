using Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

/// <summary>
/// Handles the consumption of RabbitMQ messages by deserializing the message, processing it,
/// and optionally producing a result of type <typeparamref name="TReply"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the incoming request message to be processed.</typeparam>
/// <typeparam name="TReply">The type of the response generated after processing the message.</typeparam>
public class MessageHandlerConsumer<TReply>(
    IRabbitMQClient client,
    IServiceScopeFactory scopeFactory,
    IRequestDeserializer deserializer,
    ILogger<MessageHandlerConsumer<TReply>> logger) : IMessageConsumerWithResult<TReply>
{
    public Func<TReply, Task>? OnProcessed { get; set; }
    public Func<Exception, Task>? OnError { get; set; }

    
    public async Task ProcessConsumeAsync<TRequest>(object model, BasicDeliverEventArgs ea)
    {
        logger.LogInformation($"[{ea.BasicProperties.CorrelationId}] Received message of type '{typeof(TRequest).Name}'");
        
        try
        {
            var data = deserializer.Deserialize<TRequest>(ea.Body.ToArray());
            logger.LogInformation($"[{ea.BasicProperties.CorrelationId}] Deserialized message of type '{typeof(TRequest).Name}'");

            using var scope = scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IRequestProcessor<TRequest, TReply>>();
            var product = await processor.Process(data);
            logger.LogInformation($"[{ea.BasicProperties.CorrelationId}] Processed message of type '{typeof(TRequest).Name}'");

            OnProcessed?.Invoke(product);
            await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
            logger.LogError($"[{ea.BasicProperties.CorrelationId}] Error while processing message: {e.Message}");
            OnError?.Invoke(e);
            await client.Channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
    }
}