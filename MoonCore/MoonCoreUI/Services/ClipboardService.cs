using Microsoft.JSInterop;
using MoonCoreUI.Models;

namespace MoonCoreUI.Services;

public class ClipboardService
{
    private readonly MoonCoreUiConfiguration Configuration;
    private readonly IJSRuntime JsRuntime;

    public ClipboardService(IJSRuntime jsRuntime, MoonCoreUiConfiguration configuration)
    {
        JsRuntime = jsRuntime;
        Configuration = configuration;
    }

    public async Task Copy(string content)
    {
        await JsRuntime.InvokeVoidAsync($"{Configuration.ClipboardJavascriptPrefix}.copy", content);
    }
}