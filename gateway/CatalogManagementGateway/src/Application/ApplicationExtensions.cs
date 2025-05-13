using CatalogManagementGateway.Application.Deserializers;
using CatalogManagementGateway.Hosted;
using CatalogManagementService.Application;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using Core.DTO;
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
    
    public static IServiceCollection AddDeserializers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageDeserializer<byte[], RequestReply<ProductDTO>>, RequestDeserializer<RequestReply<ProductDTO>>>();
        services.AddSingleton<IMessageSerializer<UpdateProductRequest, byte[]>, RequestSerializer<UpdateProductRequest>>(); 
        services.AddSingleton<IMessageSerializer<CreateProductRequest, byte[]>, RequestSerializer<CreateProductRequest>>();
        services.AddSingleton<IMessageSerializer<RemoveProductRequest, byte[]>, RequestSerializer<RemoveProductRequest>>();
        return services;
    }
}