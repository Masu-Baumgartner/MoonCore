using System.Reflection;
using System.Runtime.Loader;

namespace MoonCore.Plugins;

public interface IPluginSource
{
    public Task Load(AssemblyLoadContext loadContext, List<string> entrypoints);
}