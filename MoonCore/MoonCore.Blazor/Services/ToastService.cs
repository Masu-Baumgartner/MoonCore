using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MoonCore.Blazor.Components;
using MoonCore.Blazor.Components.Toasts;
using MoonCore.Blazor.Extensions;

namespace MoonCore.Blazor.Services;

public class ToastService
{
    private readonly IJSRuntime JsRuntime;
    private ToastLaunchPoint LaunchPoint;

    public ToastService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    private async Task NotificationToast(string text, string color, string icon)
    {
        await Launch<NotificationToast>(buildAttributes: parameters =>
        {
            parameters.Add("Text", text);
            parameters.Add("Color", color);
            parameters.Add("Icon", icon);
        });
    }

    public async Task Success(string text) => await NotificationToast(text, "success", "bx-check");
    
    public async Task Info(string text) => await NotificationToast(text, "info", "bx-info-circle");
    
    public async Task Warning(string text) => await NotificationToast(text, "warning", "bx-error");
    
    public async Task Danger(string text) => await NotificationToast(text, "danger", "bx-error-alt");

    public async Task Show(string id)
    {
        await JsRuntime.InvokeVoidAsyncHandled("mooncore.blazor.toasts.initAndShow", id);
    }

    public async Task Launch<T>(TimeSpan? duration = null, Action<Dictionary<string, object>>? buildAttributes = null)
        where T : ComponentBase
    {
        if (LaunchPoint == null)
            throw new ArgumentNullException(nameof(LaunchPoint),
                "You need to have a launch point initialized before using the Launch() function");

        await LaunchPoint.Launch<T>(duration, buildAttributes);
    }
        

    public Task SetLaunchPoint(ToastLaunchPoint launchPoint)
    {
        LaunchPoint = launchPoint;
        return Task.CompletedTask;
    }
}