using System.Text;
using System.Text.Json;
using CatalogManagementService.Application;
using Xunit;

namespace CatalogManagementService.tests.Deserializers;

public class RemoveProductRequestDeserializerTests
{
    private readonly RemoveProductRequestDeserializer _deserializer = new();
    
    [Fact]
    public void Process_CorrectData_ReturnsCorrectProduct()
    {
        var data = Guid.NewGuid();
        var bytes = Encoding.UTF8.GetBytes(data.ToString());
        var result = _deserializer.Deserialize(bytes);
        
        Assert.Equal(data, result);
    }

    [Theory]
    [InlineData("Product")]
    [InlineData("FakeData")]
    [InlineData("product")]
    public void Process_IncorrectData_ThrowsException(string data)
    {
        var serialized = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(serialized);
        
        Assert.Throws<FormatException>(() => _deserializer.Deserialize(bytes));
    }
}