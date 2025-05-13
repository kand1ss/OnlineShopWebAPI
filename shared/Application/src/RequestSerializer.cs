using System.Text;
using System.Text.Json;
using Core.Contracts;

namespace CatalogManagementGateway.Application.Deserializers;

public class RequestSerializer<T> : IMessageSerializer<T, byte[]>
{
    public byte[] Serialize(T data)
    {
        var json = JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }
}