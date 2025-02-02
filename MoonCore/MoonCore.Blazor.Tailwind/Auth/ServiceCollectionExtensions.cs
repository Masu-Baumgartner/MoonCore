using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Blazor.Tailwind.Auth;

public static class ServiceCollectionExtensions
{
    public static void AddAuthenticationStateManager<T>(this IServiceCollection serviceCollection) where T : AuthenticationStateManager
    {
        serviceCollection.AddScoped<T>();
        serviceCollection.AddScoped<AuthenticationStateManager>(sp => sp.GetRequiredService<T>());
        serviceCollection.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationStateManager>());
    }
}