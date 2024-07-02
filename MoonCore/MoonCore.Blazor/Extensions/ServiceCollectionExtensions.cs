using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.Forms.FastForms.Components;
using MoonCore.Blazor.Models;
using MoonCore.Blazor.Models.FastForms;
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
        collection.AddScoped<CookieService>();
        
        // Register fast form default components
        DefaultComponentRegistry.Register<int, IntComponent>();
        DefaultComponentRegistry.Register<string, StringComponent>();
        DefaultComponentRegistry.Register<double, DoubleComponent>();
        DefaultComponentRegistry.Register<decimal, DecimalComponent>();
        DefaultComponentRegistry.Register<float, FloatComponent>();
        DefaultComponentRegistry.Register<long, LongComponent>();
        DefaultComponentRegistry.Register<bool, CheckboxComponent>();
    }
}