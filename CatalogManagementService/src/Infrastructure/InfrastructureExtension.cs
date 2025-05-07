using CatalogManagementService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogManagementService.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Default"));
        });
        
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}