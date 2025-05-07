using System.Text.Json;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class ProductDTODeserializer : IMessageDeserializer<string, ProductDTO>
{
    public ProductDTO Deserialize(string data)
    {
        var result = JsonSerializer.Deserialize<ProductDTO>(data);;
        if (result is null)
            throw new ArgumentNullException(nameof(data));

        return result;
    }
}