using MoonCore.Blazor.FlyonUi.Files.Drop;

namespace MoonCore.Blazor.FlyonUi.Files;

public class DropHandlerService
{
    private DropHandler Handler;

    public bool IsEnabled => Handler.IsEnabled;
    
    public async Task Enable() => await Handler.Enable();
    public async Task Disable() => await Handler.Disable();

    public void SetHandler(DropHandler handler) => Handler = handler;
}