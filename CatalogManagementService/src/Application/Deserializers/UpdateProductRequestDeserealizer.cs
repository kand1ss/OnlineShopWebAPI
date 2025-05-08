using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.DTO;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class UpdateProductRequestDeserializer : IMessageDeserializer<byte[], UpdateProductRequest>
{
    public UpdateProductRequest Deserialize(byte[] data)
    {
        var serialized = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<UpdateProductRequest>(serialized);
        if (result is null)
            throw new ArgumentNullException(nameof(data));

        return result;
    }
}