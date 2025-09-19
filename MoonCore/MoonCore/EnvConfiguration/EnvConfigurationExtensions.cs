using Microsoft.Extensions.Configuration;

namespace MoonCore.EnvConfiguration;

public static class EnvConfigurationExtensions
{
    /// <summary>
    /// Read configuration options from the environment variables with a custom separator
    /// </summary>
    /// <param name="builder">Configuration builder to add the provider to</param>
    /// <param name="prefix"><b>Optional:</b> Prefix of all environment variables. Defaults to an empty string</param>
    /// <param name="separator"><b>Optional:</b> Separator between the configuration sections. Defaults to <b>_</b></param>
    /// <returns></returns>
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