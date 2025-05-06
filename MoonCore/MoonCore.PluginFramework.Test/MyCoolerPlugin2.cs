namespace MoonCore.PluginFramework.Test;

public class MyCoolerPlugin2 : IPlugin
{
    public void HelloWorld()
    {
        Console.WriteLine("Testy");
    }

    public async Task Kms()
    {
        Console.WriteLine(nameof(MyCoolerPlugin2));

        await Task.Delay(5000);
    }
    
    public Task<int> Why() => Task.FromResult(1);
}