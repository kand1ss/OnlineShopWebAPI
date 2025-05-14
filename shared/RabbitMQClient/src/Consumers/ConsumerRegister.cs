using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Consumers;

public class ConsumerRegister(IRabbitMQClient client) : IConsumerRegister
{
    public async Task RegisterConsumer<TRequest, TReply>(IMessageConsumerWithResult<TReply> messageConsumer, 
        string consumeQueue, CancellationToken ct = default)
    {
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        consumer.ReceivedAsync += messageConsumer.ProcessConsumeAsync<TRequest>;
        
        await client.Channel.BasicConsumeAsync(consumeQueue, false, consumer, cancellationToken: ct);
    }
}