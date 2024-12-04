using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers;

public static class ComponentHelper
{
    public static RenderFragment FromType(Type type, Action<Dictionary<string, object>>? buildAttributes = null) => builder =>
    {
        builder.OpenComponent(0, type);

        if (buildAttributes != null)
        {
            Dictionary<string, object> parameters = new();
            buildAttributes.Invoke(parameters);
            builder.AddMultipleAttributes(1, parameters);
        }
        
        builder.CloseComponent();
    };

    public static RenderFragment FromType<T>(Action<Dictionary<string, object>>? buildAttributes = null) where T : ComponentBase =>
        FromType(typeof(T), buildAttributes);
    
    public static async Task<string> RenderComponent<T>(IServiceProvider serviceProvider, Action<Dictionary<string, object>>? onConfigureParameters = null) where T : ComponentBase
    {
        var parameters = new Dictionary<string, object>();
        onConfigureParameters?.Invoke(parameters);
        
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, serviceProvider.GetRequiredService<ILoggerFactory>());

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var parameterView = ParameterView.FromDictionary(parameters!);
            var output = await htmlRenderer.RenderComponentAsync<T>(parameterView);

            return output.ToHtmlString();
        });

        return html;
    }

    public static string? GetRouteOfComponent<T>(params object[] parameters) where T : ComponentBase
    {
        var routeAttrUrl = GetRouteOfComponent<T>();

        if (routeAttrUrl == null)
            return null;

        var lastIndex = 0;

        for (var i = 0; i < parameters.Length; i++)
        {
            var start = routeAttrUrl.IndexOf('{', lastIndex);
            
            if(start == -1)
                break;
            
            var end = routeAttrUrl.IndexOf('}', start);
            lastIndex = end;
            
            if(end == -1)
                break;

            var placeholder = routeAttrUrl.Substring(start, end - start + 1);
            routeAttrUrl = routeAttrUrl.Replace(placeholder, parameters[i].ToString());
        }

        return routeAttrUrl;
    }
    
    public static string? GetRouteOfComponent<T>() where T : ComponentBase
    {
        var componentType = typeof(T);
        var routeAttrType = typeof(RouteAttribute);

        var attributes = componentType.GetCustomAttributes(false);
        var routeAttr = attributes.FirstOrDefault(x => x.GetType() == routeAttrType);

        if (routeAttr is not RouteAttribute castedRouteAttr)
            return null;
        
        return castedRouteAttr.Template;
    }
}