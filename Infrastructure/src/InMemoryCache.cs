using Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure;

public class InMemoryCache(IMemoryCache cache) : ICacheService
{
    private readonly HashSet<string> _keys = new();
    
    public void Add<T>(string key, T value)
    {
        if (_keys.Add(key))
            cache.Set(key, value);
        else
            throw new ArgumentException($"Key '{key}' already exists.");
    }

    public void Remove(string key)
        => _keys.Remove(key);

    public T? Get<T>(string key)
        => cache.Get<T>(key);

    public IEnumerable<T> GetAll<T>()
    {
        foreach (var key in _keys)
        {
            var item = cache.Get<T>(key);
            if (item != null)
                yield return item;
        }
    }

    public void Clear()
    {
        foreach(var key in _keys)
            cache.Remove(key);
        
        _keys.Clear();
    }

    public IEnumerable<string> GetAllKeys()
        => _keys;
}