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
}