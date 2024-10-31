using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.PluginFramework.Configuration;

public class InterfaceConfiguration
{
    public List<InterfaceDetails> Interfaces { get; set; } = new();
    public List<Assembly> Assemblies { get; set; } = new();

    public void AddInterface<T>(ServiceLifetime scope = ServiceLifetime.Singleton, bool useDi = true)
    {
        var type = typeof(T);

        if (!type.IsInterface)
            throw new ArgumentException("This method only accepts interfaces");
        
        Interfaces.Add(new InterfaceDetails()
        {
            Scope = scope,
            UseDi = useDi,
            Type = typeof(T)
        });
    }

    public void AddAssembly(Assembly assembly)
        => Assemblies.Add(assembly);
    
    public void AddAssemblies(IEnumerable<Assembly> assembly)
        => Assemblies.AddRange(assembly);

    public struct InterfaceDetails
    {
        public Type Type { get; set; }
        public ServiceLifetime Scope { get; set; }
        public bool UseDi { get; set; }
    }
}