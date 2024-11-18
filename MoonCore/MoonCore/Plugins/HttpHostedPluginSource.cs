using System.Runtime.Loader;

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
        
    }
}