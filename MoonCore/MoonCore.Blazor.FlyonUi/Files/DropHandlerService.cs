using MoonCore.Blazor.FlyonUi.Files.Drop;

namespace MoonCore.Blazor.FlyonUi.Files;

public class DropHandlerService
{
    private DropHandler Handler;

    public event Func<Task>? OnDropped;
    
    public bool IsEnabled => Handler.IsEnabled;
    
    public async Task Enable() => await Handler.Enable();
    public async Task Disable() => await Handler.Disable();
    public async Task<DropData?> PeekItem() => await Handler.PeekItem();
    public async Task PopItem() => await Handler.PopItem();

    public void SetHandler(DropHandler handler) => Handler = handler;

    internal async Task TriggerDropped()
    {
        if(OnDropped == null)
            return;
        
        await OnDropped.Invoke();
    }
}