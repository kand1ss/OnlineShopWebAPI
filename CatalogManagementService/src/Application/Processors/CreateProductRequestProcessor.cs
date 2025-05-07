using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class CreateProductRequestProcessor(IProductRepository repository) 
    : IRequestProcessor<ProductDTO, Product>
{
    public async Task<Product> Process(ProductDTO data)
    {
        var product = data.ToDomain();
        await repository.CreateAsync(product);
        return product;
    }
}