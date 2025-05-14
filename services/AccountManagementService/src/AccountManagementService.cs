using CatalogService.Domain;
using Core;
using Core.DTO;
using Core.Requests.Accounts;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace AccountManagementService;

public class AccountManagementService(
    IRabbitMQClient client,
    IConsumerRegister consumerRegister,
    MessageHandlerConsumer<AccountDTO> requestConsumer,
    IConnectionService connectionService,
    ILogger<AccountManagementService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Initialize(stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogCritical($"Error in service: '{e.Message}'");
        }
    }

    private async Task Initialize(CancellationToken ct = default)
    {
        await connectionService.ConnectWithRetriesAsync(client, ct);
        await InitializeConsumers(ct);
    }

    private async Task InitializeConsumers(CancellationToken ct = default)
    {
        await RegisterConsumer<CreateAccountRequest>(requestConsumer,
            GlobalQueues.CreateAccount, GlobalRoutingKeys.AccountCreated, ct);
        await RegisterConsumer<UpdateAccountRequest>(requestConsumer,
            GlobalQueues.UpdateAccount, GlobalRoutingKeys.AccountUpdated, ct);
        await RegisterConsumer<RemoveAccountRequest>(requestConsumer,
            GlobalQueues.RemoveAccount, GlobalRoutingKeys.AccountRemoved, ct);
    }

    private async Task RegisterConsumer<TRequest>(
        IMessageConsumerWithResult<AccountDTO> messageConsumer, 
        string consumeQueue,
        string publishRoutingKey,
        CancellationToken ct = default)
    {
        var consumerResultPublisher = new ResultPublishingConsumerDecorator<AccountDTO>(
            client, messageConsumer, publishRoutingKey, GlobalExchanges.Accounts);
        var consumerReplyPublisher = new ReplyPublishingMessageConsumerDecorator<AccountDTO>(
            client, consumerResultPublisher);
        
        await consumerRegister.RegisterConsumer<TRequest, AccountDTO>(consumerReplyPublisher, consumeQueue, ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.CloseConnectionAsync(cancellationToken);
    }
}