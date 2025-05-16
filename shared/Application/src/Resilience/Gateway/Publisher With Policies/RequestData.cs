namespace AccountGateway.Domain;

public class RequestData(string requestId, string exchange, string routingKey, byte[] body)
{
    public string RequestId { get; } = requestId;
    public string RoutingKey { get; } = routingKey;
    public string Exchange { get; } = exchange;
    public byte[] Body { get; } = body;
}