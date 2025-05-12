using APIGate.Application.Validators;
using CatalogManagementService.Application.DTO;
using Xunit;

namespace CatalogManagementService.tests.Deserializers;

public class RequestValidatorTests
{
    private readonly RequestValidator _validator = new();

    [Theory]
    [InlineData("Product", "Description", 10)]
    [InlineData("Title", "", 1)]
    [InlineData("Product", "", 100)]
    public void Validate_ValidObject_ReturnsTrue(string title, string? description, decimal price)
    {
        var request = new CreateProductRequest(title, description, price);
        var result = _validator.Validate(request, out var errors);
        
        Assert.True(result);
        Assert.Empty(errors);
    }
    
    [Theory]
    [InlineData("", "Description", 10)]
    [InlineData("Product", "Description", 0)]
    [InlineData("Soup", "", 100)]
    [InlineData("", "", 0)]
    public void Validate_InvalidObject_ReturnsFalse(string title, string? description, decimal price)
    {
        var request = new CreateProductRequest(title, description, price);
        var result = _validator.Validate(request, out var errors);
        
        Assert.False(result);
        Assert.NotEmpty(errors);
    }
}