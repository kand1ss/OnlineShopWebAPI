using CatalogManagementService.Application.DTO;
using Core.Contracts;
using Core.DTO;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRequestDeserializers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageDeserializer<byte[], CreateProductRequest>, RequestDeserializer<CreateProductRequest>>();
        services.AddSingleton<IMessageDeserializer<byte[], UpdateProductRequest>, RequestDeserializer<UpdateProductRequest>>();
        services.AddSingleton<IMessageDeserializer<byte[], RemoveProductRequest>, RequestDeserializer<RemoveProductRequest>>();

        return services;
    }
    
    public static IServiceCollection InitializeRequestProcessors(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessor<CreateProductRequest, ProductDTO>, CreateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<UpdateProductRequest, ProductDTO>, UpdateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<RemoveProductRequest, ProductDTO>, RemoveProductRequestProcessor>();

        return services;
    }
    
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        
        return services;
    }
}