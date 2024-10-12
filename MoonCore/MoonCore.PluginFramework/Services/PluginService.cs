using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using MoonCore.PluginFramework.Abstractions;

namespace MoonCore.PluginFramework.Services;

public class PluginService<T> where T : BasePlugin
{
    private readonly ILogger<PluginService<T>> Logger;

    private readonly List<Assembly> PluginAssemblies = new();
    private readonly List<Assembly> LibraryAssemblies = new();
    private readonly List<T> ActivePlugins = new();

    private readonly Type PluginType;
    private readonly ILoggerFactory LoggerFactory;

    public PluginService(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger<PluginService<T>>();
        PluginType = typeof(T);
    }

    public async Task Load(Func<AssemblyLoadContext, Task> loadFunction)
    {
        var context = new AssemblyLoadContext(null);

        await loadFunction.Invoke(context);

        await Load(
            context.Assemblies.ToArray()
        );
    }

    public async Task Load(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
            await Load(assembly);
    }

    public async Task Load(Assembly assembly)
    {
        // Prepare init
        var typesToInstantiate = new List<Type>();

        // Check assembly for plugins
        var pluginTypes = assembly
            .ExportedTypes
            .Where(x => x.IsSubclassOf(PluginType))
            .ToArray();

        if (pluginTypes.Length == 0)
        {
            Logger.LogInformation("Loaded {name} as library", assembly.FullName);
            LibraryAssemblies.Add(assembly);
        }
        else
        {
            typesToInstantiate.AddRange(pluginTypes);
            PluginAssemblies.Add(assembly);
        }

        // Instantiate plugins
        foreach (var type in typesToInstantiate)
        {
            try
            {
                var plugin = Activator.CreateInstance(type, [
                    LoggerFactory.CreateLogger(type)
                ]) as T;

                ActivePlugins.Add(plugin!);

                Logger.LogInformation("Instantiated plugin '{name}'", type.FullName);
            }
            catch (Exception e)
            {
                Logger.LogWarning("An error occured while instantiating plugin '{name}': {e}", type.FullName, e);
            }
        }
    }

    public T[] GetActivePlugins() => ActivePlugins.ToArray();

    public async Task Call(Func<T, Task> callFunc)
    {
        foreach (var plugin in ActivePlugins)
            await callFunc.Invoke(plugin);
    }
}