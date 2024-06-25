namespace MoonCore.Helpers;

public class DynamicStorage
{
    private readonly Dictionary<string, object> Data = new();

    public void Set(string id, object data)
    {
        Data[id] = data;
    }

    public T Get<T>(string id)
    {
        return (T)Get(id)!;
    }

    public bool ContainsKey(string key) => Data.ContainsKey(key);
    
    public T? GetNullable<T>(string id)
    {
        var res = Get(id);

        return res == null ? default : (T)res;
    }

    public T Get<T>()
    {
        var searchType = typeof(T);

        if (Data.Any(x => x.Value.GetType() == searchType))
            return (T)Data.First(x => x.Value.GetType() == searchType).Value;

        return default!;
    }

    public object? Get(string id)
    {
        if (!Data.TryGetValue(id, out var value))
            return null;

        return value;
    }

    public void Clear()
    {
        Data.Clear();
    }
}