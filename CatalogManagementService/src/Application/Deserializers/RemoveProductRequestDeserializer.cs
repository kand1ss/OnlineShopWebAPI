using System.Text;
using System.Text.Json;
using Core.Contracts;
using Core.DTO;

namespace CatalogManagementService.Application;

public class RemoveProductRequestDeserializer : IMessageDeserializer<byte[], Guid>
{
    public Guid Deserialize(byte[] data)
    {
        var serialized = Encoding.UTF8.GetString(data);
        return Guid.Parse(serialized);
    }
}