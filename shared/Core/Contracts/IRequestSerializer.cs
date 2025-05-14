namespace Core.Contracts;

public interface IRequestSerializer<out TResult>
{
    TResult Serialize<T>(T data);
}