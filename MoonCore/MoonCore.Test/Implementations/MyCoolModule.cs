using Microsoft.Extensions.Logging;
using MoonCore.Test.Interfaces;

namespace MoonCore.Test.Implementations;

public class MyCoolModule : IAppStartup
{
    private readonly ILogger<MyCoolModule> Logger;

    public MyCoolModule(ILogger<MyCoolModule> logger)
    {
        Logger = logger;
    }

    public void BuildWebApplication()
    {
        Logger.LogInformation("I am doing my part to build");
    }

    public void ConfigureWebApplication()
    {
        Logger.LogInformation("I am doing my part to configure");
    }
}