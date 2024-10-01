using Microsoft.Extensions.Logging;

namespace MoonCore.PluginFramework.Abstractions;

public abstract class BasePlugin
{
    public ILogger<BasePlugin> Logger { get; set; }
    
    protected BasePlugin(ILogger<BasePlugin> logger)
    {
        Logger = logger;
    }
}