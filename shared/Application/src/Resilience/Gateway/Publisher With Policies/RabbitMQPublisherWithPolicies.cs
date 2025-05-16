using AccountGateway.Application;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using RabbitMQ.Client;
using RabbitMQClient;

namespace Application.Resilience.Gateway.Publisher_With_Policies;

public class RabbitMQPublisherWithPolicies(
    IRabbitMQClient client, 
    AsyncPolicyWrap policyWrap,
    ILogger<RabbitMQPoliciesWrap> logger)
{
    public async Task PublishAsync(string requestId, string exchange, string routingKey, byte[] body)
    {
        var context = new Context
        {
            ["body"] = body,
            ["routingKey"] = routingKey,
            ["requestId"] = requestId,
            ["exchange"] = exchange
        };

        await policyWrap.ExecuteAsync(async (ctx) =>
        {
            var props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                CorrelationId = requestId
            };

            await client.Channel.BasicPublishAsync(exchange, routingKey, true, props, body);
            logger.LogInformation("Message published");
        }, context);
    }
}