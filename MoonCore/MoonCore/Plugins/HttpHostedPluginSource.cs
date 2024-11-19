using System.Runtime.Loader;
using MoonCore.Extensions;
using MoonCore.Models;

namespace MoonCore.Plugins;

public class HttpHostedPluginSource : IPluginSource
{
    private readonly string Url;

    public HttpHostedPluginSource(string url)
    {
        Url = url;
    }

    public async Task Load(AssemblyLoadContext loadContext, List<string> entrypoints)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(Url);
        await response.HandlePossibleApiError();

        var manifest = await response.ParseAsJson<HostedPluginsManifest>();

        foreach (var assembly in manifest.Assemblies)
        {
            var pluginStream = await httpClient.GetStreamAsync($"{Url}/stream?assembly={assembly}");
            loadContext.LoadFromStream(pluginStream);
        }
        
        entrypoints.AddRange(manifest.Entrypoints);
    }
}