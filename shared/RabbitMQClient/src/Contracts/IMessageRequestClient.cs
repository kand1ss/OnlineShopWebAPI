using CatalogManagementService.Application.Replies;

namespace RabbitMQClient.Contracts;

public interface IMessageRequestClient<TReply>
{
    Task<RequestReply<TReply>> PublishMessageAndConsumeReply(
        byte[] body, string publishRoutingKey, string replyQueue, TimeSpan? timeout = null);
}