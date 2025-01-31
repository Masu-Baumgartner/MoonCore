using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.HotKeys;

public class HotKeyService
{
    private readonly IJSRuntime JsRuntime;
    private readonly List<HotKey> HotKeys = new();

    public HotKeyService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Register(string key, ModifierKey modifierKey, Func<Task> onPressed)
    {
        // Load already existing hot key, so we can reuse it
        var hotKey = HotKeys.FirstOrDefault(x => x.Key == key && x.Modifier == modifierKey);

        if (hotKey == null)
        {
            // Hot key model doesn't exist, lets create it
            hotKey = new HotKey()
            {
                Key = key,
                Modifier = modifierKey
            };

            // Safe it to a list to keep track of it
            HotKeys.Add(hotKey);

            // And let the client know
            await JsRuntime.InvokeVoidAsync(
                "moonCoreKeyBinds.registerHotkey",
                hotKey.Key,
                GetModifierString(modifierKey),
                hotKey.GetHashCode().ToString(),
                DotNetObjectReference.Create(this)
            );
        }
        
        // Subscribe to the event, increase the listener count
        hotKey.OnPressed += onPressed;
        hotKey.ListenerCount++;
    }

    public async Task Unregister(string key, ModifierKey modifierKey, Func<Task> onPressed)
    {
        // Check if such a hot key does event exist
        var hotKey = HotKeys.FirstOrDefault(x => x.Key == key && x.Modifier == modifierKey);
        
        if(hotKey == null) // Do nothing when its already deleted
            return;

        // Unsubscribe and decrease the listener counter
        hotKey.OnPressed -= onPressed;
        hotKey.ListenerCount--;

        if (hotKey.ListenerCount == 0) // CHeck if all listeners are gone
        {
            // Let the client know that we no longer need that hot key as all listeners are gone for it 
            await JsRuntime.InvokeVoidAsync(
                "moonCoreKeyBinds.unregisterHotkey",
                hotKey.Key,
                GetModifierString(modifierKey)
            );
            
            // Remove the model, as we no longer need to keep track of it
            HotKeys.Remove(hotKey);
        }
    }

    [JSInvokable]
    public async Task OnHotkeyPressed(string action)
    {
        var hotKey = HotKeys.FirstOrDefault(x => x.GetHashCode().ToString() == action);
        
        if(hotKey == null)
            return;

        await hotKey.Trigger();
    }

    private string? GetModifierString(ModifierKey modifierKey)
    {
        return modifierKey switch
        {
            ModifierKey.Alt => "alt",
            ModifierKey.Control => "ctrl",
            ModifierKey.Meta => "meta",
            ModifierKey.Shift => "shift",
            ModifierKey.None => null,
            _ => null
        };
    }
}