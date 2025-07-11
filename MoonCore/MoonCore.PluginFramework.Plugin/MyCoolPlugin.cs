namespace MoonCore.PluginFramework.Plugin;

public class MyCoolPlugin : ICoolPlugin
{
    public MyCoolPlugin()
    {
        Console.WriteLine("AAAAAAAAAAAAAA");
    }

    public void PrintName()
    {
        Console.WriteLine(nameof(MyCoolPlugin));
    }

    public Task Kms()
    {
        Console.WriteLine("BBBBBBB");
        return Task.CompletedTask;
    }
}