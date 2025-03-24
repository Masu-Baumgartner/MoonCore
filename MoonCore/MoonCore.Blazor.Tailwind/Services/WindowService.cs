using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Services;

public class WindowService
{
    private readonly IJSRuntime JsRuntime;

    public WindowService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task<int[]> GetSize()
    {
        return await JsRuntime.InvokeAsync<int[]>("moonCore.window.getSize");
    }
}