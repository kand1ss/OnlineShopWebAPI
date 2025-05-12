using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure.Repositories;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class UpdateProductRequestProcessor(IProductRepository repository, ILogger<UpdateProductRequestProcessor> logger) 
    : IRequestProcessor<UpdateProductRequest, ProductDTO>
{
    public async Task<ProductDTO> Process(UpdateProductRequest data)
    {
        var product = await repository.GetAsync(data.Id);
        if (product is null)
            throw new InvalidOperationException($"UPDATE: Product with id '{data.Id}' not found.");
        
        if (!string.IsNullOrEmpty(data.Title))
            product.Title = data.Title;
        if (!string.IsNullOrEmpty(data.Description))
            product.Description = data.Description;
        if (data.Price > 0)
            product.Price = data.Price;

        await repository.UpdateAsync(product);
        
        logger.LogInformation($"Product with id '{product.Id}' updated.");
        return product.ToDTO();
    }
}