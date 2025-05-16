using CatalogManagementService.Application.DTO;
using Core.Contracts;
using Core.DTO;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQClient;
using RabbitMQClient.Consumers;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRequestProcessors(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessor<CreateProductRequest, ProductDTO>, CreateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<UpdateProductRequest, ProductDTO>, UpdateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<RemoveProductRequest, ProductDTO>, RemoveProductRequestProcessor>();

        return services;
    }

    public static IServiceCollection InitializeRequestConsumers(this IServiceCollection services)
    {
        services.AddSingleton<MessageHandlerConsumer<ProductDTO>>();
        
        return services;
    }
    
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        services.AddSingleton<IConsumerRegister, ConsumerRegister>();
        
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
            .WithMetrics(meter =>
            {
                meter.AddAspNetCoreInstrumentation()
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