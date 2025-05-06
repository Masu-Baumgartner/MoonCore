namespace MoonCore.PluginFramework.Test;

public class MyPlugin1 : IPlugin
{
    public void HelloWorld()
    {
        Console.WriteLine("Elo world 1");
    }

    public Task Kms()
    {
        Console.WriteLine(nameof(MyPlugin1));
        return Task.CompletedTask;
    }

    public Task<int> Why() => Task.FromResult(1);
}