namespace MoonCore.PluginFramework.Test;

public interface IPlugin
{
    public void HelloWorld();
    public Task Kms();
    public Task<int> Why();
}