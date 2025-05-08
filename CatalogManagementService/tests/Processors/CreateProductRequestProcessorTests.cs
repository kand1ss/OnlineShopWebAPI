using CatalogManagementService.Application;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Infrastructure;
using CatalogManagementService.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace CatalogManagementService.tests;

public class CreateProductRequestProcessorTests : TestWhichUsingInMemoryDb
{
    private readonly CreateProductRequestProcessor _processor;

    public CreateProductRequestProcessorTests()
    {
        var repository = new ProductRepository(Context);
        _processor = new(
            repository, new Mock<ILogger<CreateProductRequestProcessor>>().Object);
    }
    
    [Theory]
    [InlineData("Product", null, 100)]
    [InlineData("Another Product", "Some description", 0)]
    [InlineData("Third Product", "", 0.01)]
    public async Task Process_DifferentRequestValues_CreatesProductWithCorrectProperties(string title, string? description, decimal price)
    {
        var request = new CreateProductRequest(title, description, price);
        var result = await _processor.Process(request);

        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.Equal(price, result.Price);
    }
}