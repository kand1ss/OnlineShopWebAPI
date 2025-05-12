using Core;

namespace CatalogManagementService.Infrastructure.Repositories;

public interface IProductRepository
{
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<Product?> GetAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
}