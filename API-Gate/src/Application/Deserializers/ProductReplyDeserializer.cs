using System.Text;
using System.Text.Json;
using API_Gate;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using Core.DTO;

namespace APIGate.Application.Deserializers;

public class ProductReplyDeserializer : IMessageDeserializer<byte[], RequestReply<ProductDTO>>
{
    public RequestReply<ProductDTO> Deserialize(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        var reply = JsonSerializer.Deserialize<RequestReply<ProductDTO>>(json);
        if (reply is null)
            throw new ArgumentNullException(nameof(data));
        
        return reply;
    }
}