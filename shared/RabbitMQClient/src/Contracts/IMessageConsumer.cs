using RabbitMQ.Client.Events;

namespace RabbitMQClient.Contracts;

public interface IMessageConsumer
{
    Task ProcessConsumeAsync<TRequest>(object model, BasicDeliverEventArgs ea);
}