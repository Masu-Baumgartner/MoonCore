using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers;

/// <summary>
/// Static helper class for creating render fragments of components and/or rendering them to html content
/// </summary>
public static class ComponentHelper
{
    /// <summary>
    /// Creates a render fragment from a component type with optionally parameters
    /// </summary>
    /// <param name="type">Type of the component</param>
    /// <param name="buildAttributes"><b>Optional:</b> Parameters for the component</param>
    /// <returns><see cref="RenderFragment"/> of the component</returns>
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

    /// <summary>
    ///  Creates a render fragment from a component type with optionally parameters
    /// </summary>
    /// <param name="buildAttributes">><b>Optional:</b> Parameters for the component</param>
    /// <typeparam name="T">Type of the component</typeparam>
    /// <returns><see cref="RenderFragment"/> of the component</returns>
    public static RenderFragment FromType<T>(Action<Dictionary<string, object>>? buildAttributes = null) where T : ComponentBase =>
        FromType(typeof(T), buildAttributes);
    
    /// <summary>
    /// Renders a component to a html string
    /// </summary>
    /// <param name="serviceProvider">Service provider to create the render instance and inject all other necessary services</param>
    /// <param name="onConfigureParameters"><b>Optional:</b> Callback to configure the parameters of the component</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>HTML string of the rendered component</returns>
    public static async Task<string> RenderToHtmlAsync<T>(IServiceProvider serviceProvider, Action<Dictionary<string, object>>? onConfigureParameters = null) where T : ComponentBase
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