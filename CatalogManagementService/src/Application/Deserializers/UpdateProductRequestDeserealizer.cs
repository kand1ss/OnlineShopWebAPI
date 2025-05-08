using System.Text.Json;
using CatalogManagementService.Application.DTO;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class UpdateProductRequestDeserializer : IMessageDeserializer<string, UpdateProductRequest>
{
    public UpdateProductRequest Deserialize(string data)
    {
        var result = JsonSerializer.Deserialize<UpdateProductRequest>(data);;
        if (result is null)
            throw new ArgumentNullException(nameof(data));

        return result;
    }
}