using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessor<ProductDTO, Product>, CreateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<ProductDTO, Product>, UpdateProductRequestProcessor>();
        services.AddScoped<IRequestProcessor<ProductDTO, Product>, RemoveProductRequestProcessor>();
        
        return services;
    }
}