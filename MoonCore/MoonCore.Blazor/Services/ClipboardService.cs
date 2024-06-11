using Microsoft.JSInterop;

namespace MoonCore.Blazor.Services;

public class ClipboardService
{
    private readonly IJSRuntime JsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Copy(string content)
    {
        
    }
}