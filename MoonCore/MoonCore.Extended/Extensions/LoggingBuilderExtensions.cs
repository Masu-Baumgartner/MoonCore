using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoonCore.Helpers;
using MoonCore.Logging;
using MoonCore.Models;

namespace MoonCore.Extensions;

public static class LoggingBuilderExtensions
{
    public static void AddMoonCore(this ILoggingBuilder builder, Action<MoonCoreLoggingConfiguration>? configureAction = null)
    {
        builder.ClearProviders();

        var configuration = new MoonCoreLoggingConfiguration();
        
        if(configureAction != null)
            configureAction.Invoke(configuration);

        builder.AddProviders(LoggerBuildHelper.BuildFromConfiguration(configuration));
    }
}