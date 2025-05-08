using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.DTO;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class CreateProductRequestDeserializer : IMessageDeserializer<byte[], CreateProductRequest>
{
    public CreateProductRequest Deserialize(byte[] data)
    {
        var serialized = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<CreateProductRequest>(serialized);
        if (result is null)
            throw new ArgumentNullException(nameof(data));

        return result;
    }
}