using MoonCore.Attributes;

namespace MoonCore.Test;

[RunBefore(typeof(Class2))]
public class Class3 : IStartupLayer
{
    public Task Run()
    {
        Console.WriteLine("3");
        return Task.CompletedTask;
    }
}