using Microsoft.JSInterop;
using MoonCoreUI.Extensions;
using MoonCoreUI.Models;

namespace MoonCoreUI.Services;

public class AlertService
{
    private readonly MoonCoreUiConfiguration Configuration;
    private readonly IJSRuntime JsRuntime;

    public AlertService(IJSRuntime jsRuntime, MoonCoreUiConfiguration configuration)
    {
        JsRuntime = jsRuntime;
        Configuration = configuration;
    }

    public async Task Info(string title, string message = "")
    {
        await JsRuntime.InvokeVoidAsyncHandled($"{Configuration.AlertJavascriptPrefix}.info", title, message);
    }
    
    public async Task Success(string title, string message = "")
    {
        await JsRuntime.InvokeVoidAsyncHandled($"{Configuration.AlertJavascriptPrefix}.success", title, message);
    }
    
    public async Task Warning(string title, string message = "")
    {
        await JsRuntime.InvokeVoidAsyncHandled($"{Configuration.AlertJavascriptPrefix}.warning", title, message);
    }
    
    public async Task Error(string title, string message = "")
    {
        await JsRuntime.InvokeVoidAsyncHandled($"{Configuration.AlertJavascriptPrefix}.error", title, message);
    }
    
    public async Task<string> Text(string title, string message = "", string initialValue = "")
    {
        return await JsRuntime.InvokeAsyncHandled<string>($"{Configuration.AlertJavascriptPrefix}.text", title, message, initialValue);
    }
    
    public async Task<bool> YesNo(string title, string yes = "Yes", string no = "No")
    {
        try
        {
            return await JsRuntime.InvokeAsyncHandled<bool>($"{Configuration.AlertJavascriptPrefix}.yesno", title, yes, no);
        }
        catch (Exception)
        {
            return false;
        }
    }
}