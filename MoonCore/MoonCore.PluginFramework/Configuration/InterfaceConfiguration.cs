using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.PluginFramework.Configuration;

public class InterfaceConfiguration
{
    public Dictionary<Type, ServiceLifetime> Interfaces { get; set; } = new();
    public List<Assembly> Assemblies { get; set; } = new();

    public void AddInterface<T>(ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        var type = typeof(T);

        if (!type.IsInterface)
            throw new ArgumentException("This method only accepts interfaces");
        
        Interfaces[type] = scope;
    }

    public void AddAssembly(Assembly assembly)
        => Assemblies.Add(assembly);
    
    public void AddAssemblies(IEnumerable<Assembly> assembly)
        => Assemblies.AddRange(assembly);
}