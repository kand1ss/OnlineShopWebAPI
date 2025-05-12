using APIGate.Application.Deserializers;
using APIGate.Hosted;
using CatalogManagementService.Application;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using Core.DTO;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace APIGate.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
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