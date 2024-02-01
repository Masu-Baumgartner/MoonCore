using Microsoft.JSInterop;

namespace MoonCoreUI.Services;

public class ClipboardService
{
    public static string Prefix { get; set; } = "mooncore";
    
    private readonly IJSRuntime JsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Copy(string content)
    {
        await JsRuntime.InvokeVoidAsync($"{Prefix}.copy", content);
    }
}