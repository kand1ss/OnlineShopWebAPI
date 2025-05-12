using System.Text;
using System.Text.Json;
using Core.Contracts;

namespace CatalogManagementService.Application;

public class RequestDeserializer<T> : IMessageDeserializer<byte[], T>
{
    public T Deserialize(byte[] data)
    {
        var serialized = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<T>(serialized)
            ?? throw new ArgumentNullException(nameof(data));

        return result;
    }
}