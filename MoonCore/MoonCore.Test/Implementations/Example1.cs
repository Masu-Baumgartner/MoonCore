using Microsoft.Extensions.Logging;
using MoonCore.Test.Interfaces;

namespace MoonCore.Test.Implementations;

public class Example1 : IExample
{
    private readonly ExampleA ExampleA;
    private readonly ILogger<Example1> Logger;

    public Example1(ExampleA exampleA, ILogger<Example1> logger)
    {
        ExampleA = exampleA;
        Logger = logger;
    }

    public void DoSmth()
    {
        Logger.LogInformation("Calling A");
        ExampleA.X();
    }
}