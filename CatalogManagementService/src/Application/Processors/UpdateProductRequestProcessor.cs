using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class UpdateProductRequestProcessor(IProductRepository repository, ILogger<UpdateProductRequestProcessor> logger) 
    : IRequestProcessor<UpdateProductRequest, Product>
{
    public async Task<Product> Process(UpdateProductRequest data)
    {
        var product = await repository.GetAsync(Guid.Parse(data.Id));
        if (product is null)
            throw new InvalidOperationException($"UPDATE: Product with id '{data.Id}' not found.");
        
        product.Title = data.Title;
        product.Description = data.Description;
        product.Price = data.Price;

        await repository.UpdateAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' updated.");
        return product;
    }
}