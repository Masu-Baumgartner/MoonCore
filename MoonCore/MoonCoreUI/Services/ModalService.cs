using Microsoft.JSInterop;
using MoonCoreUI.Models;

namespace MoonCoreUI.Services;

public class ModalService
{
    private readonly MoonCoreUiConfiguration Configuration;
    private readonly IJSRuntime JsRuntime;

    public ModalService(IJSRuntime jsRuntime, MoonCoreUiConfiguration configuration)
    {
        JsRuntime = jsRuntime;
        Configuration = configuration;
    }

    public async Task Show(string id, bool focus = true) // Focus can be specified to fix issues with other components
    {
        try
        {
            await JsRuntime.InvokeVoidAsync($"{Configuration.ModalJavascriptPrefix}.show", id, focus);
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    public async Task Hide(string id)
    {
        try
        {
            await JsRuntime.InvokeVoidAsync($"{Configuration.ModalJavascriptPrefix}.hide", id);
        }
        catch (Exception)
        {
            // Ignored
        }
    }
}