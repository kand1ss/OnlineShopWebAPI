using System.Text;
using RabbitMQ.Client.Events;

namespace RabbitMQClient;

public static class MessageExtensions
{
    public static string GetBodyAsString(this BasicDeliverEventArgs ea)
        => Encoding.UTF8.GetString(ea.Body.ToArray());
}