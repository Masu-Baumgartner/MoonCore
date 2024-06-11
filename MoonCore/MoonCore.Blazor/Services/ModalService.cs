using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MoonCore.Blazor.Components;

namespace MoonCore.Blazor.Services;

public class ModalService
{
    private readonly IJSRuntime JsRuntime;
    private ModalLaunchPoint? LaunchPoint = null;

    public ModalService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Show(string id, bool focus = true) // Focus can be specified to fix issues with other components
    {
        await JsRuntime.InvokeVoidAsync("mooncore.blazor.modals.show", id, focus);
    }
    
    public async Task Hide(string id)
    {
        await JsRuntime.InvokeVoidAsync("mooncore.blazor.modals.hide", id);
    }

    public Task SetLaunchPoint(ModalLaunchPoint launchPoint)
    {
        LaunchPoint = launchPoint;
        return Task.CompletedTask;
    }

    public async Task Launch<T>(bool focus = false, string cssClasses = "",
        Action<Dictionary<string, object>>? buildAttributes = null) where T : ComponentBase
    {
        if (LaunchPoint == null)
            throw new ArgumentNullException(nameof(LaunchPoint),
                "You need to have a launch point initialized before using the Launch() function");

        await LaunchPoint.Launch<T>(focus, cssClasses, buildAttributes);
    }
}