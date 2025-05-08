using CatalogManagementService.Application;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure;
using Core;
using Moq;
using Xunit;

namespace CatalogManagementService.tests;

public class UpdateProductRequestProcessorTests : TestWhichUsingInMemoryDb
{
    private readonly ProductRepository _repository;
    private readonly UpdateProductRequestProcessor _processor;

    public UpdateProductRequestProcessorTests()
    {
        _repository = new ProductRepository(Context);
        _processor = new(
            _repository,
            new Mock<ILogger<UpdateProductRequestProcessor>>().Object);
    }

    [Theory]
    [InlineData("Product2", "Description", 100)]
    [InlineData("Product3", null, 150)]
    [InlineData("Product", null, 150)]
    [InlineData("Product", "Description", 150)]
    public async Task Process_ExistingProduct_ProductUpdated(string title, string? description, decimal price)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Description = null,
            Price = 100,
        };
        var request = new UpdateProductRequest(product.Id, title, description, price);
        
        await _repository.CreateAsync(product);
        var result = await _processor.Process(request);
        
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Price, result.Price);
        
        var productFromRepo = await _repository.GetAsync(request.Id);
        
        Assert.NotNull(productFromRepo);
        Assert.Equal(request.Id, productFromRepo.Id);
        Assert.Equal(request.Title, productFromRepo.Title);
        Assert.Equal(request.Description, productFromRepo.Description);
        Assert.Equal(request.Price, productFromRepo.Price);
    }

    [Fact]
    public async Task Process_NonExistingProduct_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _processor.Process(new UpdateProductRequest(Guid.NewGuid(), "Product", "Description", 100)));
    }
}