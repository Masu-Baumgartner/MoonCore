using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.Tailwind.Alerts;
using MoonCore.Blazor.Tailwind.Modals;
using MoonCore.Blazor.Tailwind.Services;
using MoonCore.Blazor.Tailwind.Toasts;

namespace MoonCore.Blazor.Tailwind.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMoonCoreBlazorTailwind(this IServiceCollection collection)
    {
        collection.AddScoped<ModalService>();
        collection.AddScoped<ToastService>();
        collection.AddScoped<AlertService>();
        collection.AddScoped<QueryParameterService>();
    }
}