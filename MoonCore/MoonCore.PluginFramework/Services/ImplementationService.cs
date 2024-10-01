namespace MoonCore.PluginFramework.Services;

public class ImplementationService
{
    private readonly Dictionary<Type, List<object>> Implementations = new();
    
    public void Register<T>(T implementation) where T : class
    {
        var interfaceType = typeof(T);

        lock (Implementations)
        {
            if(!Implementations.ContainsKey(interfaceType))
                Implementations.Add(interfaceType, new());

            Implementations[interfaceType].Add(implementation);
        }
    }

    public void Register<T, TImpl>() where TImpl : T where T : class
    {
        var impl = Activator.CreateInstance<TImpl>();

        Register<T>(impl);
    }

    public T[] Get<T>() where T : class
    {
        var interfaceType = typeof(T);

        lock (Implementations)
        {
            if (!Implementations.TryGetValue(interfaceType, out var implementation))
                return [];

            return implementation
                .Select(x => (T)x)
                .ToArray();
        }
    }

    public async Task Execute<T>(Func<T, Task> func) where T : class
    {
        var implementations = Get<T>();

        foreach (var implementation in implementations)
            await func.Invoke(implementation);
    }
}