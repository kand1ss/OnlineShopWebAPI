using CatalogManagementService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CatalogManagementService.tests;

public class TestWhichUsingInMemoryDb
{
    protected CatalogDbContext _context;

    public TestWhichUsingInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CatalogDbContext(options);
    }
}