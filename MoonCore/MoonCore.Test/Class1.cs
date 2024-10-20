using MoonCore.Attributes;

namespace MoonCore.Test;

public class Class1 : IStartupLayer
{
    public Task Run()
    {
        Console.WriteLine("1");
        return Task.CompletedTask;
    }
}