using Microsoft.Extensions.DependencyInjection;
using MoonCoreUI.Models;

namespace MoonCoreUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMoonCoreUi(this IServiceCollection collection, Action<MoonCoreUiConfiguration>? configuration = null)
    {
        MoonCoreUiConfiguration config = new();
        
        if(configuration != null)
            configuration.Invoke(config);

        collection.AddSingleton(config);
    }
}