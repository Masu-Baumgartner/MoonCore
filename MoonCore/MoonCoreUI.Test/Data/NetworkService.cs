using MoonCore.Helpers;

namespace MoonCoreUI.Test.Data;

public class NetworkService
{
    public NetworkEvent<string> OnConsoleMessage { get; }

    public readonly NetworkEventConnection Connection = new();

    public NetworkService()
    {
        OnConsoleMessage = new("console.message", Connection);
    }
}