using Microsoft.Extensions.Configuration;

namespace MoonCore.EnvConfiguration;

public static class EnvConfigurationExtensions
{
    public static IConfigurationBuilder AddEnvironmentVariables(this IConfigurationBuilder builder, string? prefix, string? separator)
    {
        builder.Add(new EnvConfigurationSource()
        {
            Prefix = prefix,
            Separator = separator
        });

        return builder;
    }
}