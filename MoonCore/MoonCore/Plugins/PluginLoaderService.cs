using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;

namespace MoonCore.Plugins;

public class PluginLoaderService
{
    public Assembly[] Assemblies { get; private set; }
    public string[] Entrypoints { get; private set; }

    private readonly ILogger<PluginLoaderService> Logger;
    private readonly List<IPluginSource> Sources = new();

    public PluginLoaderService(ILogger<PluginLoaderService> logger)
    {
        Logger = logger;
    }

    public void AddSource(IPluginSource source)
        => Sources.Add(source);

    public async Task Load()
    {
        try
        {
            var loadContext = new AssemblyLoadContext(null);
            var entrypoints = new List<string>();

            foreach (var source in Sources)
            {
                try
                {
                    await source.Load(loadContext, entrypoints);
                }
                catch (Exception e)
                {
                    Logger.LogError(
                        "An unhandled error occured while using source {name} to load plugins: {e}",
                        source.GetType().FullName,
                        e
                    );
                }
            }

            Entrypoints = entrypoints.ToArray();
            Assemblies = loadContext.Assemblies.ToArray();
            
            Logger.LogTrace("Loaded {assemblyCount} assemblies with {entrypointCount} entrypoints", Assemblies.Length, Entrypoints.Length);
        }
        catch (Exception e)
        {
            Logger.LogError("An unhandled error occured while loading plugins from sources: {e}", e);
        }
    }
}