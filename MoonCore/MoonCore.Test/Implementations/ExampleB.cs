using Microsoft.Extensions.Logging;
using MoonCore.Test.Interfaces;

namespace MoonCore.Test.Implementations;

public class ExampleB : IExample
{
    private readonly ILogger<ExampleB> Logger;

    public ExampleB(ILogger<ExampleB> logger)
    {
        Logger = logger;
    }

    public void DoSmth()
    {
        Logger.LogInformation("I am an example :)");
    }
}