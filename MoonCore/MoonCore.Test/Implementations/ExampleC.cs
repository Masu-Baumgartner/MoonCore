using Microsoft.Extensions.Logging;
using MoonCore.Test.Interfaces;

namespace MoonCore.Test.Implementations;

public class ExampleC : IExample
{
    private readonly ILogger<ExampleC> Logger;
    private readonly Example2? Example2;

    public ExampleC(ILogger<ExampleC> logger, Example2? example2 = null)
    {
        Logger = logger;
        Example2 = example2;
    }

    public void DoSmth()
    {
        Logger.LogInformation("C found dependent service: {res}", Example2 != null);
    }
}