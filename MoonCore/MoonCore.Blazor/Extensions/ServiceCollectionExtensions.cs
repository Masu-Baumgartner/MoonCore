using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.Models;
using MoonCore.Blazor.Services;

namespace MoonCore.Blazor.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMoonCoreBlazor(this IServiceCollection collection, Action<MoonCoreBlazorConfiguration>? configuration = null)
    {
        MoonCoreBlazorConfiguration config = new();
        
        if(configuration != null)
            configuration.Invoke(config);

        collection.AddSingleton(config);
        
        // Configure interop services
        collection.AddScoped<ModalService>();
        collection.AddScoped<AlertService>();
        collection.AddScoped<ToastService>();
        collection.AddScoped<DownloadService>();
        collection.AddScoped<ClipboardService>();
    }
}