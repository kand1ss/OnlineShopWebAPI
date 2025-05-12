using System.Text.Json;
using API_Gate;
using APIGate.Application;
using APIGate.Application.Parsers;
using APIGate.Application.Validators;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Application.Replies;
using Core;
using Core.Contracts;
using Core.DTO;
using Grpc.Core;

namespace APIGate.Services;

public class CatalogManagementService(
    MessageRequestClient<ProductDTO> messageRequestClient,
    IMessageSerializer<CreateProductRequest, byte[]> createRequestSerializer,
    IMessageSerializer<UpdateProductRequest, byte[]> updateRequestSerializer,
    IMessageSerializer<RemoveProductRequest, byte[]> removeRequestSerializer,
    ProductRequestValidator validator,
    ILogger<CatalogManagementService> logger) 
    : API_Gate.CatalogManagementService.CatalogManagementServiceBase
{
    public override async Task<ProductReply> CreateProduct(CreateProductData request, ServerCallContext context)
    {
        logger.LogInformation("CREATE: Received a request to create a product.");
        if (!ProductRequestParser.TryParseDecimal(request.Price, out var price, out var error))
            return CreateReply(false, "", error);
        
        var requestData = new CreateProductRequest(request.Title, request.Description, price);
        var validationResult = TryValidate(requestData, out var reply);
        if (!validationResult)
            return reply!;
        
        var body = createRequestSerializer.Serialize(requestData);
        logger.LogInformation("CREATE: The request was successfully deserialized.");
        
        var response = await messageRequestClient.PublishMessageAndConsumeReply(body, 
            GlobalQueues.CreateProduct, GlobalQueues.ProductOperationReply);

        return ConvertReply(response);
    }

    private ProductReply ConvertReply(RequestReply<ProductDTO> reply)
    {
        var serializableResult = "";
        if (reply.Success)
            serializableResult = JsonSerializer.Serialize(reply.Result);
        
        return new ProductReply { Success = reply.Success, Result = serializableResult, Error = reply.Error ?? "" };
    }
    
    private bool TryValidate<T>(T request, out ProductReply? reply)
    {
        var result = validator.Validate(request, out var errors);
        reply = CreateReply(result, "", string.Join("; ", errors));

        return result;
    }

    private static ProductReply CreateReply(bool success, string result, string error)
        => new() { Success = success, Result = result, Error = error };

    public override async Task<ProductReply> UpdateProduct(UpdateProductData request, ServerCallContext context)
    {
        logger.LogInformation("UPDATE: Received a request to update a product.");
        if (!ProductRequestParser.TryParseDecimal(request.Price, out var price, out var error)
            || !ProductRequestParser.TryParseGuid(request.Id, out var guid, out error))
            return CreateReply(false, "", error);

        var requestData = new UpdateProductRequest(guid, request.Title, request.Description, price);
        var validationResult = TryValidate(requestData, out var reply);
        if (!validationResult)
            return reply!;

        var body = updateRequestSerializer.Serialize(requestData);
        logger.LogInformation("UPDATE: The request was successfully deserialized.");
        
        var response = await messageRequestClient.PublishMessageAndConsumeReply(body, 
            GlobalQueues.UpdateProduct, GlobalQueues.ProductOperationReply);

        return ConvertReply(response);
    }

    public override async Task<ProductReply> RemoveProduct(RemoveProductData request, ServerCallContext context)
    {
        logger.LogInformation("REMOVE: Received a request to remove a product.");
        if(!ProductRequestParser.TryParseGuid(request.Id, out var guid, out var error))
            return CreateReply(false, "", error);
        
        var requestData = new RemoveProductRequest(guid);
        var body = removeRequestSerializer.Serialize(requestData);
        var response = await messageRequestClient.PublishMessageAndConsumeReply(body, 
            GlobalQueues.RemoveProduct, GlobalQueues.ProductOperationReply);

        return ConvertReply(response);
    }
}