using System.Text;
using System.Text.Json;
using Core.Contracts;

namespace APIGate.Application.Deserializers;

public class ProductRequestSerializer<T> : IMessageDeserializer<T, byte[]>
{
    public byte[] Deserialize(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }
}