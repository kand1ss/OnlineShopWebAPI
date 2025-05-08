using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class CreateProductRequestProcessor(IProductRepository repository, ILogger<CreateProductRequestProcessor> logger) 
    : IRequestProcessor<CreateProductRequest, Product>
{
    public async Task<Product> Process(CreateProductRequest data)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = data.Title,
            Description = data.Description,
            Price = data.Price,
        };
        
        await repository.CreateAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' created.");
        return product;
    }
}