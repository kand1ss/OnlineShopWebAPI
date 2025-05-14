namespace Core.Contracts;

public interface IRequestDeserializer
{
    T Deserialize<T>(byte[] data);
}