using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestProcessor(IProductRepository repository, ILogger<RemoveProductRequestProcessor> logger) 
    : IRequestProcessor<Guid, ProductDTO>
{
    public async Task<ProductDTO> Process(Guid data)
    {
        var product = await repository.GetAsync(data);
        if (product is null)
            throw new InvalidOperationException($"DELETE: Product with id '{data}' not found.");

        await repository.DeleteAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' removed.");
        return product.ToDTO();
    }
}