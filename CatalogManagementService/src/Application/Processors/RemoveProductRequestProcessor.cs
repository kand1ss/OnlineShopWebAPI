using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestProcessor(IProductRepository repository) : IRequestProcessor<ProductDTO, Product>
{
    public async Task<Product> Process(ProductDTO data)
    {
        var product = data.ToDomain();
        await repository.DeleteAsync(product);
        return product;
    }
}