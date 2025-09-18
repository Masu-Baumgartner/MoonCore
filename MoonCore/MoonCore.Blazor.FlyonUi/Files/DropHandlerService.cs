using MoonCore.Blazor.FlyonUi.Files.Drop;

namespace MoonCore.Blazor.FlyonUi.Files;

/// <summary>
/// Handles the communication with the <see cref="DropHandler"/>. To use this service you
/// need a <see cref="DropHandler"/> instance somewhere in your UI
/// </summary>
public class DropHandlerService
{
    private DropHandler Handler;

    /// <summary>
    /// Event which gets invoked whenever files have been dropped
    /// </summary>
    public event Func<Task>? OnDropped;
    
    /// <summary>
    /// Shows if the file drop handler is currently enabled
    /// </summary>
    public bool IsEnabled => Handler.IsEnabled;
    
    /// <summary>
    /// Enables the drop handler
    /// </summary>
    public async Task EnableAsync() => await Handler.EnableAsync();
    
    /// <summary>
    /// Disables the drop handler
    /// </summary>
    public async Task DisableAsync() => await Handler.DisableAsync();
    
    /// <summary>
    /// Peeks at the next item in the stack of files provided by the interop
    /// </summary>
    /// <returns><b>null</b> if no item is left otherwise it returns the next item</returns>
    public async Task<DropData?> PeekItemAsync() => await Handler.PeekItemAsync();
    
    /// <summary>
    /// Pops the current item of the interop stack
    /// </summary>
    public async Task PopItemAsync() => await Handler.PopItemAsync();

    internal void SetHandler(DropHandler handler) => Handler = handler;

    internal async Task TriggerDroppedAsync()
    {
        if(OnDropped == null)
            return;
        
        await OnDropped.Invoke();
    }
}