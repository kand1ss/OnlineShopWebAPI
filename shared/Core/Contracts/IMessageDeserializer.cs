namespace Core.Contracts;

public interface IMessageDeserializer<in T, out TResult>
{
    TResult Deserialize(T data);
}