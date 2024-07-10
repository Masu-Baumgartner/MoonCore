using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.Tailwind.Services;

namespace MoonCore.Blazor.Tailwind.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMoonCoreBlazorTailwind(this IServiceCollection collection)
    {
        collection.AddScoped<ModalService>();
        //collection.AddScoped<AlertService>();
        collection.AddScoped<ToastService>();
    }
}