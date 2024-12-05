using Microsoft.AspNetCore.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Extensions;

public static class NavigationManagerExtensions
{
    public static void NavigateTo<T>(this NavigationManager navigationManager) where T : ComponentBase
    {
        var url = ComponentHelper.GetRouteOfComponent<T>();
        
        if(string.IsNullOrEmpty(url))
            return;
        
        navigationManager.NavigateTo(url);
    }
    
    public static void NavigateTo<T>(this NavigationManager navigationManager, params object[] parameters) where T : ComponentBase
    {
        var url = ComponentHelper.GetRouteOfComponent<T>(parameters);
        
        if(string.IsNullOrEmpty(url))
            return;
        
        navigationManager.NavigateTo(url);
    }
}