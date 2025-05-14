using CatalogManagementGateway.Application.Deserializers;
using CatalogManagementGateway.Hosted;
using CatalogManagementService.Application;
using Core.Contracts;
using OpenTelemetry.Trace;
using RabbitMQClient.Contracts;

namespace RabbitMQClient.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
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
            });
        
        return services;
    }
}