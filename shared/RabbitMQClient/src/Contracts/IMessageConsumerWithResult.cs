namespace RabbitMQClient.Contracts;

public interface IMessageConsumerWithResult<TResult> : IMessageConsumer
{
    Func<TResult, Task>? OnProcessed { get; set; }
    Func<Exception, Task>? OnError { get; set; }
}