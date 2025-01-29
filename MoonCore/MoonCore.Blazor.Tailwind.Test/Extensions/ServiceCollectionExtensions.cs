using Microsoft.AspNetCore.Components.Authorization;
using MoonCore.Blazor.Tailwind.Auth;
using MoonCore.Blazor.Tailwind.Test.Models;

namespace MoonCore.Blazor.Tailwind.Test.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAuthenticationStateManager<T>(this IServiceCollection serviceCollection) where T : AuthenticationStateManager
    {
        serviceCollection.AddScoped<T>();
        serviceCollection.AddScoped<AuthenticationStateManager>(sp => sp.GetRequiredService<T>());
        serviceCollection.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationStateManager>());
    }
}