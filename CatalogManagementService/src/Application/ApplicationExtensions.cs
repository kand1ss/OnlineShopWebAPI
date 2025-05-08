using CatalogManagementService.Application.DTO;
using Core;
using Core.Contracts;
using RabbitMQClient;
using RabbitMQClient.Contracts;

namespace CatalogManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessor<CreateProductRequest, Product>, CreateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<UpdateProductRequest, Product>, UpdateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<Guid, Product>, RemoveProductRequestProcessor>();
        
        services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
        services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
        
        return services;
    }
}