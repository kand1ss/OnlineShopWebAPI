using System.Text;
using System.Text.Json;
using CatalogManagementService.Application;
using CatalogManagementService.Application.DTO;
using Core.DTO;
using Xunit;

namespace CatalogManagementService.tests.Deserializers;

public class CreateProductRequestDeserializerTests
{
    private readonly CreateProductRequestDeserializer _deserializer = new();
    
    [Theory]
    [InlineData("product", null, 100)]
    [InlineData("Product2", "Description", 10)]
    [InlineData("Product", null, 0)]
    public void Process_CorrectData_ReturnsCorrectProduct(string title, string? description, decimal price)
    {
        var request = new CreateProductRequest(title, description, price);
        var serialized = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(serialized);
        
        var result = _deserializer.Deserialize(bytes);
        
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);;
        Assert.Equal(price, result.Price);
    }

    [Theory]
    [InlineData("Product")]
    [InlineData("FakeData")]
    [InlineData("product")]
    public void Process_IncorrectData_ThrowsException(string data)
    {
        var serialized = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        Assert.Throws<JsonException>(() => _deserializer.Deserialize(bytes));
    }
}