using System.Text;
using System.Text.Json;
using API_Gate;
using APIGate.Application.Validators;
using APIGate.Consumers;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Application.Replies;
using Core;
using Core.Contracts;
using Grpc.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient;

namespace APIGate.Services;

public class CatalogManagementService(
    IRabbitMQClient client,
    IMessageDeserializer<byte[], ProductOperationReply> replyDeserializer,
    ProductRequestValidator validator) 
    : API_Gate.CatalogManagementService.CatalogManagementServiceBase
{
    private readonly string _replyQueue = GlobalQueues.ProductOperationReply;
    
    public override async Task<ProductReply> CreateProduct(CreateProductData request, ServerCallContext context)
    {
        if (!TryParseDecimal(request.Price, out var price, out var error))
            return CreateReply(error, false);
        
        var requestData = new CreateProductRequest(request.Title, request.Description, price);
        var validationResult = TryValidate(requestData, out var reply);
        if (!validationResult)
            return reply!;
        
        var body = GetBytesFrom(requestData);
        return await PublishMessageAndConsumeReply(body, GlobalQueues.CreateProduct);
    }
    
    private static bool TryParseDecimal(string? value, out decimal result, out string error)
    {
        if (decimal.TryParse(value, out result))
        {
            error = "";
            return true;
        }

        error = "Price must be a valid decimal number";
        return false;
    }
    
    private bool TryValidate<T>(T request, out ProductReply? reply)
    {
        var result = validator.Validate(request, out var errors);
        reply = CreateReply(string.Join("; ", errors), result);

        return result;
    }

    private async Task<ProductReply> PublishMessageAndConsumeReply(byte[] body, string publishRoutingKey)
    {
        var reply = CreateReply("No response was received", false);
        var generatedId = Guid.NewGuid().ToString();
        var props = CreateBasicProperties(generatedId);

        await client.Channel.BasicPublishAsync("", publishRoutingKey, true, props, body);
        
        var tcs = new TaskCompletionSource<ProductReply>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        var replyConsumer = new ProductReplyConsumer(client, replyDeserializer, generatedId, tcs);
        
        consumer.ReceivedAsync += replyConsumer.ProcessConsumeAsync;
        await client.Channel.BasicConsumeAsync(_replyQueue, false, consumer);
        
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(15)));
        if (completedTask == tcs.Task)
            reply = tcs.Task.Result;
        
        if (consumer.ConsumerTags.Length > 0)
            await client.Channel.BasicCancelAsync(consumer.ConsumerTags.First());
        
        return reply;
    }

    private BasicProperties CreateBasicProperties(string generatedId)
    {
        return new BasicProperties
        {
            CorrelationId = generatedId,
            ReplyTo = _replyQueue
        };
    }

    private static byte[] GetBytesFrom<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }

    private static ProductReply CreateReply(string message, bool success)
        => new() { Message = message, Success = success };

    public override async Task<ProductReply> UpdateProduct(UpdateProductData request, ServerCallContext context)
    {
        if (!TryParseDecimal(request.Price, out var price, out var error))
            return CreateReply(error, false);
        
        var requestData = new UpdateProductRequest(Guid.Parse(request.Id), request.Title, request.Description, price);
        var validationResult = TryValidate(requestData, out var reply);
        if (!validationResult)
            return reply!;
        
        var body = GetBytesFrom(requestData);
        return await PublishMessageAndConsumeReply(body, GlobalQueues.UpdateProduct);
    }

    public override async Task<ProductReply> RemoveProduct(RemoveProductData request, ServerCallContext context)
    {
        var body = Encoding.UTF8.GetBytes(request.Id);
        return await PublishMessageAndConsumeReply(body, GlobalQueues.RemoveProduct);
    }
}