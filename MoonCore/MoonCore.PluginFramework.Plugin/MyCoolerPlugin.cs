namespace MoonCore.PluginFramework.Plugin;

public class MyCoolerPlugin : ICoolPlugin
{
    public void PrintName()
    {
        Console.WriteLine(nameof(MyCoolerPlugin));
    }

    public Task Kms()
    {
        Console.WriteLine("KMS 1");
        return Task.CompletedTask;
    }
}