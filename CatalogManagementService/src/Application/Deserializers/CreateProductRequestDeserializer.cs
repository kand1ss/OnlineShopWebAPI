using System.Text.Json;
using CatalogManagementService.Application.DTO;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class CreateProductRequestDeserializer : IMessageDeserializer<string, CreateProductRequest>
{
    public CreateProductRequest Deserialize(string data)
    {
        var result = JsonSerializer.Deserialize<CreateProductRequest>(data);;
        if (result is null)
            throw new ArgumentNullException(nameof(data));

        return result;
    }
}