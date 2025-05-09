using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.Replies;
using Core.Contracts;

namespace APIGate.Application.Deserializers;

public class ProductReplyDeserializer : IMessageDeserializer<byte[], ProductOperationReply>
{
    public ProductOperationReply Deserialize(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        var reply = JsonSerializer.Deserialize<ProductOperationReply>(json);
        if (reply is null)
            throw new ArgumentNullException(nameof(data));
        
        return reply;
    }
}