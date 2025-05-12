using System.Text;
using System.Text.Json;
using APIGate.Application.Deserializers;
using CatalogManagementService.Application.DTO;
using Xunit;

namespace CatalogManagementService.tests.Deserializers;

public class RequestSerializerTests
{
    private readonly RequestSerializer<CreateProductRequest> _deserializer = new();
    
    [Theory]
    [InlineData("product", null, 100)]
    [InlineData("Product2", "Description", 10)]
    [InlineData("Product", null, 0)]
    public void Process_CorrectData_ReturnsCorrectProduct(string title, string? description, decimal price)
    {
        var request = new CreateProductRequest(title, description, price);
        var serialized = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(serialized);
        
        var result = _deserializer.Serialize(request);
        Assert.Equal(result, bytes);
    }
}