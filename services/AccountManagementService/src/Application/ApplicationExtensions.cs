using AccountManagementService.Application.Interfaces;
using AccountManagementService.Application.Processors;
using Core.Contracts;
using Core.DTO;
using Core.Requests.Accounts;
using RabbitMQClient.Consumers;

namespace AccountManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddRequestProcessors(this IServiceCollection services)
    {
        services.AddSingleton<IAccountUpdater, AccountUpdater>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IRequestProcessor<CreateAccountRequest, AccountDTO>, CreateAccountRequestProcessor>();
        services.AddScoped<IRequestProcessor<UpdateAccountRequest, AccountDTO>, UpdateAccountRequestProcessor>();
        services.AddScoped<IRequestProcessor<RemoveAccountRequest, AccountDTO>, RemoveAccountRequestProcessor>();
        
        return services;
    }
    
    public static IServiceCollection AddRequestConsumers(this IServiceCollection services)
    {
        services.AddSingleton<MessageHandlerConsumer<AccountDTO>>();
        return services;
    }
}