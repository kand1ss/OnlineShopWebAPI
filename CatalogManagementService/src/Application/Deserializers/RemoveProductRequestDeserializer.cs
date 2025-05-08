using System.Text.Json;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestDeserializer : IMessageDeserializer<string, Guid>
{
    public Guid Deserialize(string data)
    {
        var result = JsonSerializer.Deserialize<Guid>(data);
        return result;
    }
}