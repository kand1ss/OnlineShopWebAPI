using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.Replies;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

/// <summary>
/// A decorator class that adds reply publishing functionality to an existing
/// <see cref="IMessageConsumerWithResult{TReply}"/> implementation. This class ensures that
/// a reply is published back to the message sender after the message is processed.
/// </summary>
/// <typeparam name="TReply">The type of the reply message to be returned after processing.</typeparam>
public class ReplyPublishingMessageConsumerDecorator<TReply>(
    IRabbitMQClient client, 
    IMessageConsumerWithResult<TReply> consumer) 
    : IMessageConsumerWithResult<TReply>
{
    public Func<TReply, Task>? OnProcessed { get; set; }
    public Func<Exception, Task>? OnError { get; set; }

    public async Task ProcessConsumeAsync<TRequest>(object model, BasicDeliverEventArgs ea)
    {
        async Task OnConsumerOnProcessed(TReply reply)
        {
            await SendReply(ea.BasicProperties, RequestReply<TReply>.Ok(reply));
            OnProcessed?.Invoke(reply);
        }

        try
        {
            consumer.OnProcessed += OnConsumerOnProcessed;
            await consumer.ProcessConsumeAsync<TRequest>(model, ea);
        }
        catch (Exception e)
        {
            await SendReply(ea.BasicProperties, RequestReply<TReply>.Fail(e.Message));
            OnError?.Invoke(e);
            throw;
        }
        finally
        {
            consumer.OnProcessed -= OnConsumerOnProcessed;
        }
    }
    
    private async Task SendReply(IReadOnlyBasicProperties requestProperties, RequestReply<TReply> reply)
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