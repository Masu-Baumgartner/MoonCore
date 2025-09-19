using Microsoft.Extensions.Configuration;

namespace MoonCore.Yaml;

public static class YamlConfigurationExtensions
{
    /// <summary>
    /// Adds yaml support for the <see cref="IConfigurationBuilder"/>
    /// </summary>
    /// <param name="builder">Builder to add yaml support to</param>
    /// <param name="path">Path of the yaml file</param>
    /// <param name="isOptional">Specifies if the file is optional. If set to false this will throw an exception when missing</param>
    /// <param name="prefix">Prefix to add to all read sections</param>
    public static void AddYamlFile(this IConfigurationBuilder builder, string path, bool isOptional = false, string? prefix = null)
    {
        builder.Add(new YamlConfigurationSource(path, isOptional, prefix));
    }
}