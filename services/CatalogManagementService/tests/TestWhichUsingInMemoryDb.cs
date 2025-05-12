using CatalogManagementService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CatalogManagementService.tests;

public class TestWhichUsingInMemoryDb
{
    protected readonly CatalogDbContext Context;

    public TestWhichUsingInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new CatalogDbContext(options);
    }
}