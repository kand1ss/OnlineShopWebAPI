using CatalogManagementService.Application;
using CatalogManagementService.Infrastructure;
using Core;
using Moq;
using Xunit;

namespace CatalogManagementService.tests;

public class RemoveProductRequestProcessorTests : TestWhichUsingInMemoryDb
{
    private readonly RemoveProductRequestProcessor _processor;
    private readonly ProductRepository _repository;

    public RemoveProductRequestProcessorTests()
    {
        _repository = new ProductRepository(Context);
        _processor = new(
            _repository, 
            new Mock<ILogger<RemoveProductRequestProcessor>>().Object);
    }

    [Fact]
    public async Task Process_ExistingProduct_ProductRemoved()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Description = null,
            Price = 100,
        };
        
        await _repository.CreateAsync(product);
        await _processor.Process(product.Id);
        
        Assert.Empty(await _repository.GetAllAsync());
        Assert.Null(await _repository.GetAsync(product.Id));
    }
    
    [Fact]
    public async Task Process_NonExistingProduct_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _processor.Process(Guid.NewGuid()));
    }
}