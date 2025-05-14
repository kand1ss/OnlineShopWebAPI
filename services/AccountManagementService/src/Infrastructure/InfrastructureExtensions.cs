using AccountManagementService.Infrastructure.Context;
using AccountManagementService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace AccountManagementService.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        services.AddSingleton<IConsumerRegister, ConsumerRegister>();

        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        return services;
    }
}