using Microsoft.Extensions.Configuration;

namespace MoonCore.Yaml;

public static class YamlConfigurationExtensions
{
    public static void AddYamlFile(this IConfigurationBuilder builder, string path, bool isOptional = false, string? prefix = null)
    {
        builder.Add(new YamlConfigurationSource(path, isOptional, prefix));
    }
}