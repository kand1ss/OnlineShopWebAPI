using CatalogManagementService.Infrastructure.Repositories;
using Core;
using Microsoft.EntityFrameworkCore;

namespace CatalogManagementService.Infrastructure;

public class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public async Task CreateAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<Product?> GetAsync(Guid id)
        => await context.Products.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await context.Products.ToListAsync();
}