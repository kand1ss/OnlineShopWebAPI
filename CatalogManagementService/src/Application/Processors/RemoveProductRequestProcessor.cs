using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure.Repositories;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestProcessor(IProductRepository repository, ILogger<RemoveProductRequestProcessor> logger) 
    : IRequestProcessor<RemoveProductRequest, ProductDTO>
{
    public async Task<ProductDTO> Process(RemoveProductRequest data)
    {
        var product = await repository.GetAsync(data.Id);
        if (product is null)
            throw new InvalidOperationException($"DELETE: Product with id '{data}' not found.");

        await repository.DeleteAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' removed.");
        return product.ToDTO();
    }
}