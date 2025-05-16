using AccountManagementService.Application.Interfaces;
using AccountManagementService.Application.Processors;
using Core.Contracts;
using Core.DTO;
using Core.Requests.Accounts;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQClient;
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
    
    public static IServiceCollection InitializeOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddConsoleExporter();
            });
        
        return services;
    }
    
    public static IServiceCollection InitializeHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection") ?? "")
            .AddRabbitMQ(provider =>
            {
                var client = provider.GetRequiredService<IRabbitMQClient>();
                return client.Connection;
            });
            
        return services;
    }
}