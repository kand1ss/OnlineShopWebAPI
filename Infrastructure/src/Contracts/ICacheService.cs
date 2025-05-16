namespace Infrastructure.Contracts;

public interface ICacheService
{
    void Add<T>(string key, T value);
    void Remove(string key);
    T? Get<T>(string key);
    IEnumerable<T> GetAll<T>();
    void Clear();
    IEnumerable<string> GetAllKeys();
}