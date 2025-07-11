namespace MoonCore.PluginFramework.Plugin;

public class MyCoolerPlugin : ICoolPlugin
{
    public void PrintName()
    {
        Console.WriteLine(nameof(MyCoolerPlugin));
    }
}