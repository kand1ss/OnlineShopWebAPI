using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

/// <summary>
/// A decorator for an <see cref="IMessageConsumerWithResult{TReply}"/> implementation that publishes
/// the processing result to a specified RabbitMQ exchange and routing key.
/// </summary>
/// <typeparam name="TReply">
/// The type of the result produced by the consumer after processing a message.
/// </typeparam>
/// <remarks>
/// This class is used to wrap an existing message consumer and automatically publish its results
/// to a RabbitMQ exchange with a specific routing key. It ensures that after the consumer processes
/// a message, its result is handled by the <see cref="IRabbitMQClient"/> for publishing.
/// </remarks>
public class ResultPublishingConsumerDecorator<TReply>(
    IRabbitMQClient client, 
    IMessageConsumerWithResult<TReply> consumer,
    string publishRoutingKey,
    string exchange = "") : IMessageConsumerWithResult<TReply>
{
    public Func<TReply, Task>? OnProcessed { get; set; }
    public Func<Exception, Task>? OnError { get; set; }

    public async Task ProcessConsumeAsync<TRequest>(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            consumer.OnProcessed += PublishResult;
            await consumer.ProcessConsumeAsync<TRequest>(model, ea);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
        finally
        {
            consumer.OnProcessed -= PublishResult;
        }
    }
    
    private async Task PublishResult(TReply reply)
    {
        var json = JsonSerializer.Serialize(reply);
        var body = Encoding.UTF8.GetBytes(json);
        
        await client.Channel.BasicPublishAsync(exchange, publishRoutingKey, 
            true, body: body);
        OnProcessed?.Invoke(reply);
    }
}