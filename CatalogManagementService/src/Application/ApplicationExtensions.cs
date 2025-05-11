using CatalogManagementService.Application.DTO;
using Core;
using Core.Contracts;
using Core.DTO;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection InitializeRequestDeserializers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageDeserializer<byte[], CreateProductRequest>, CreateProductRequestDeserializer>();
        services.AddSingleton<IMessageDeserializer<byte[], UpdateProductRequest>, UpdateProductRequestDeserializer>();
        services.AddSingleton<IMessageDeserializer<byte[], Guid>, RemoveProductRequestDeserializer>();

        return services;
    }
    
    public static IServiceCollection InitializeRequestProcessors(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessor<CreateProductRequest, ProductDTO>, CreateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<UpdateProductRequest, ProductDTO>, UpdateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<Guid, ProductDTO>, RemoveProductRequestProcessor>();

        return services;
    }
    
    public static IServiceCollection InitializeRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        
        return services;
    }
}