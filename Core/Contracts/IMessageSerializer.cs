namespace Core.Contracts;

public interface IMessageSerializer<in T, out TResult>
{
    TResult Serialize(T data);
}