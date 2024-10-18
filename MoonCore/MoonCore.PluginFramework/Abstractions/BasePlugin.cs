using Microsoft.Extensions.Logging;

namespace MoonCore.PluginFramework.Abstractions;

public abstract class BasePlugin
{
    public ILogger Logger { get; set; }
    
    protected BasePlugin(ILogger logger)
    {
        Logger = logger;
    }
}