using System.Security.Claims;
using AccountGateway.Application;
using AccountGatewayGRPC;
using CatalogManagementGateway.Application.Validators;
using Core;
using Core.Contracts;
using Core.Requests.Accounts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using RabbitMQ.Client;
using RabbitMQClient;

namespace AccountGateway;

public class AccountGateway(
    IRequestSerializer<byte[]> requestSerializer, 
    RequestValidator validator,
    IRabbitMQClient client) : AccountGatewayGRPC.AccountGateway.AccountGatewayBase
{
    private async Task<AccountReply> ExceptionHandlingWrap(Func<Task<AccountReply>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            return CreateReply("null", e.Message, false);
        }
    }
    
    private static AccountReply CreateReply(string accountId, string message, bool success)
        => new() { AccountId = accountId, Message = message, Success = success };
    
    public override async Task<AccountReply> CreateAccount(CreateAccountData request, ServerCallContext context)
        => await ExceptionHandlingWrap(() => TryCreateAccount(request));

    private async Task<AccountReply> TryCreateAccount(CreateAccountData request)
    {
        var id = Guid.NewGuid();
        var requestData = request.MapToRequest(id);

        if(!validator.Validate(requestData, out var errors))
            return CreateReply(id.ToString(), string.Join("; ", errors), false);

        var body = requestSerializer.Serialize(requestData);
        await client.Channel.BasicPublishAsync("", GlobalQueues.CreateAccount, 
            true, body: body);

        return CreateReply(id.ToString(), "Account creation request successfully confirmed", 
            true);
    }

    public override async Task<AccountReply> UpdateAccount(UpdateAccountData request, ServerCallContext context)
        => await ExceptionHandlingWrap(() => TryUpdateAccount(request));

    private async Task<AccountReply> TryUpdateAccount(UpdateAccountData request)
    {
        var requestData = request.MapToRequest();

        if(!validator.Validate(requestData, out var errors))
            return CreateReply(request.Id, string.Join("; ", errors), false);

        var body = requestSerializer.Serialize(requestData);
        await client.Channel.BasicPublishAsync("", GlobalQueues.UpdateAccount, 
            true, body);

        return CreateReply(request.Id, "Account update request successfully confirmed", 
            true);
    }

    public override async Task<AccountReply> RemoveAccount(Empty request, ServerCallContext context)
        => await ExceptionHandlingWrap(() => TryRemoveAccount(context));

    private async Task<AccountReply> TryRemoveAccount(ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null || !Guid.TryParse(userId, out var guid))
            return CreateReply("null", "Client sent token with invalid identifier",
                false);
        
        var body = requestSerializer.Serialize(new RemoveAccountRequest(guid));
        await client.Channel.BasicPublishAsync("", GlobalQueues.RemoveAccount, 
            true, body);

        return CreateReply(userId, "Account removal request successfully confirmed", true);
    }
}