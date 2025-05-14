namespace RabbitMQClient.Contracts;

public interface IConsumerRegister
{
    Task RegisterConsumer<TRequest, TReply>(
        IMessageConsumerWithResult<TReply> messageConsumer,
        string consumeQueue,
        CancellationToken ct = default);
}