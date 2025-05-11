using Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

/// <summary>
/// Represents a message handler that processes RabbitMQ messages of type <typeparamref name="TRequest"/>
/// and produces a response of type <typeparamref name="TReply"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request message to be processed.</typeparam>
/// <typeparam name="TReply">The type of the response produced after processing the message.</typeparam>
public class MessageHandlerConsumer<TRequest, TReply>(
    IRabbitMQClient client, 
    IServiceScopeFactory scopeFactory,
    IMessageDeserializer<byte[], TRequest> deserializer) : IMessageConsumerWithResult<TReply>
{
    public Func<TReply, Task>? OnProcessed { get; set; }

    public async Task ProcessConsumeAsync(object model, BasicDeliverEventArgs ea)
    {
        var data = deserializer.Deserialize(ea.Body.ToArray());
        
        using var scope = scopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<IRequestProcessor<TRequest, TReply>>();
        var product = await processor.Process(data);

        OnProcessed?.Invoke(product);
        await client.Channel.BasicAckAsync(ea.DeliveryTag, false);
    }
}