using RabbitMQ.Client.Events;

namespace RabbitMQClient.Contracts;

public interface IMessageConsumer
{
    Task ProcessConsumeAsync(object model, BasicDeliverEventArgs ea);
}