using Microsoft.Extensions.Logging;
using MoonCore.Test.Interfaces;

namespace MoonCore.Test.Implementations;

public class ExampleA : IExample
{
    private readonly ILogger<ExampleA> Logger;

    public ExampleA(ILogger<ExampleA> logger)
    {
        Logger = logger;
    }

    public void DoSmth()
    {
        Logger.LogInformation("I am an example :>");
    }

    public void X()
    {
        Logger.LogInformation("X ;)");
    }
}