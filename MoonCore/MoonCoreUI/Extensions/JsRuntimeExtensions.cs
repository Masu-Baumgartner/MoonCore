using Microsoft.JSInterop;

namespace MoonCoreUI.Extensions;

public static class JsRuntimeExtensions
{
    public static async Task InvokeVoidAsyncHandled(this IJSRuntime jsRuntime, string function, params object[] parameter)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(function, parameter);
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }
    
    public static async Task<T> InvokeAsyncHandled<T>(this IJSRuntime jsRuntime, string function, params object[] parameter)
    {
        try
        {
            return await jsRuntime.InvokeAsync<T>(function, parameter);
        }
        catch (TaskCanceledException)
        {
            // ignored

            return default(T)!;
        }
    }
}