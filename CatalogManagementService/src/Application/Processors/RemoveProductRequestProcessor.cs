using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestProcessor(IProductRepository repository, ILogger<RemoveProductRequestProcessor> logger) 
    : IRequestProcessor<Guid, Product>
{
    public async Task<Product> Process(Guid data)
    {
        var product = await repository.GetAsync(data);
        if (product is null)
            throw new InvalidOperationException($"DELETE: Product with id '{data}' not found.");

        await repository.DeleteAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' removed.");
        return product;
    }
}