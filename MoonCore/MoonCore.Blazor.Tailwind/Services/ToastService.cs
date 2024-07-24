using MoonCore.Blazor.Tailwind.Components.Toasts;
using MoonCore.Blazor.Tailwind.Models;

namespace MoonCore.Blazor.Tailwind.Services;

public class ToastService
{
    private ToastLaunchPoint? LaunchPoint = null;

    public async Task Hide(ToastLaunchItem item)
    {
        if (LaunchPoint == null)
        {
            throw new ArgumentNullException(nameof(LaunchPoint),
                "You need to have a launch point initialized before using this function");
        }

        await LaunchPoint.Hide(item);
    }
    
    public async Task Hide(string id)
    {
        if (LaunchPoint == null)
        {
            throw new ArgumentNullException(nameof(LaunchPoint),
                "You need to have a launch point initialized before using this function");
        }

        await LaunchPoint.Hide(id);
    }

    public Task SetLaunchPoint(ToastLaunchPoint launchPoint)
    {
        LaunchPoint = launchPoint;
        return Task.CompletedTask;
    }

    public async Task Launch<T>(string? id = null, bool enableAutoDisappear = true, Action<Dictionary<string, object>>? buildAttributes = null) where T : BaseToast
    {
        if (LaunchPoint == null)
        {
            throw new ArgumentNullException(nameof(LaunchPoint),
                "You need to have a launch point initialized before using this function");
        }

        await LaunchPoint.Launch<T>(id, enableAutoDisappear, buildAttributes);
    }
}