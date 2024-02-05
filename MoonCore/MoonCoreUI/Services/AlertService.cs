using Microsoft.JSInterop;

namespace MoonCoreUI.Services;

public class AlertService
{
    public static string Prefix { get; set; } = "mooncore";
    
    private readonly IJSRuntime JsRuntime;

    public AlertService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Info(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync($"{Prefix}.info", title, message);
    }
    
    public async Task Success(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync($"{Prefix}.success", title, message);
    }
    
    public async Task Warning(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync($"{Prefix}.warning", title, message);
    }
    
    public async Task Error(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync($"{Prefix}.error", title, message);
    }
    
    public async Task<string> Text(string title, string message, string initialValue = "")
    {
        return await JsRuntime.InvokeAsync<string>($"{Prefix}.text", title, message, initialValue);
    }
    
    public async Task<bool> YesNo(string title, string yes, string no)
    {
        try
        {
            return await JsRuntime.InvokeAsync<bool>($"{Prefix}.yesno", title, yes, no);
        }
        catch (Exception)
        {
            return false;
        }
    }
}