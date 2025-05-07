using Core;
using Core.DTO;

namespace CatalogManagementService.Application;

public static class ProductMapper
{
    public static ProductDTO ToDTO(this Product product)
        => new(product.Title, product.Description, product.Price);

    public static Product ToDomain(this ProductDTO product)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CreatedUtc = DateTime.UtcNow
        };
}