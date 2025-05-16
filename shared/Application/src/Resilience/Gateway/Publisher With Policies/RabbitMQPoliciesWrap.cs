using Application.Resilience.Gateway.Publisher_With_Policies;
using Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using RabbitMQClient;
using RequestData = AccountGateway.Domain.RequestData;

namespace AccountGateway.Application;

public class RabbitMQPoliciesWrap
{
    private readonly ICacheService _cache;
    private readonly RabbitMQPublisherWithPolicies _publisher;

    private readonly ILogger<RabbitMQPoliciesWrap> _logger;
    
    public RabbitMQPoliciesWrap(
        IRabbitMQClient client, ICacheService cache, ILogger<RabbitMQPoliciesWrap> logger)
    {
        _cache = cache;
        _logger = logger;
        var policyWrap = InitializePolicy(logger);
        
        _publisher = new RabbitMQPublisherWithPolicies(client, policyWrap, logger);
    }

    private AsyncPolicyWrap InitializePolicy(ILogger<RabbitMQPoliciesWrap> logger)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt =>
            {
                var time = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                logger.LogWarning($"Attempt {attempt} of publishing message failed. Retrying in {time}...");
                return time;
            });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(30),
                onBreak: (ex, timeSpan) =>
                    logger.LogWarning($"Circuit opened for {timeSpan}"),
                onReset: async () =>
                {
                    logger.LogInformation("Circuit reset");
                    await FlushCacheAsync();
                });

        var fallbackPolicy = Policy
            .Handle<BrokenCircuitException>()
            .FallbackAsync(
                fallbackAction: async (ctx, ct) =>
                {
                    var body = (byte[])ctx["body"];
                    var routingKey = (string)ctx["routingKey"];
                    var requestId = (string)ctx["requestId"];
                    var exchange = (string)ctx["exchange"];

                    var requestData = new RequestData(requestId, exchange, routingKey, body);
                    _cache.Add(requestId, requestData);
                },
                onFallbackAsync: async (outcome, ctx) =>
                {
                    logger.LogWarning("CircuitBreaker activated. Message saved to cache.");
                    await Task.CompletedTask;
                });
        
        return Policy.WrapAsync(fallbackPolicy, retryPolicy, circuitBreakerPolicy);
    }
    
    public async Task PublishAsync(string requestId, string exchange, string routingKey, byte[] body)
        => await _publisher.PublishAsync(requestId, exchange, routingKey, body);
    
    private async Task TryPublishMessage(RequestData requestData, string key)
    {
        try
        {
            await PublishAsync(requestData.RequestId, requestData.Exchange, requestData.RoutingKey,
                requestData.Body);
            _cache.Remove(key);
        }
        catch (BrokenCircuitException e)
        {
            _logger.LogError("Circuit re-broken during flush. Re-caching remaining messages.");
        }
        catch (Exception e)
        {
            _logger.LogError("Error publishing message to queue. Re-caching remaining messages.");
        }
    }

    private async Task FlushCacheAsync()
    {
        foreach (var key in _cache.GetAllKeys())
        {
            var requestData = _cache.Get<RequestData>(key);
            if (requestData is null)
                continue;

            await TryPublishMessage(requestData, key);
        }
    }
}