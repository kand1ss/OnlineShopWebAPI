using CatalogManagementGateway.Application.Deserializers;
using CatalogManagementGateway.Hosted;
using CatalogManagementService.Application;
using Core.Contracts;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace AccountGateway.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        services.AddHostedService<RabbitMQInitializer>();
        
        return services;
    }
    
    public static IServiceCollection AddSerializers(this IServiceCollection services)
    {
        services.AddSingleton<IRequestDeserializer, RequestDeserializer>();
        services.AddSingleton<IRequestSerializer<byte[]>, RequestSerializer>(); 

        return services;
    }
    
    public static IServiceCollection InitializeOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddConsoleExporter();
            })
            .WithMetrics(meter =>
            {
                meter.AddAspNetCoreInstrumentation()
                    .AddConsoleExporter();
            });
        
        return services;
    }
}