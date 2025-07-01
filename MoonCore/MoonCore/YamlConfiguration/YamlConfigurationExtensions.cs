using Microsoft.Extensions.Configuration;

namespace MoonCore.YamlConfiguration;

public static class YamlConfigurationExtensions
{
    public static void AddYamlFile(IConfigurationBuilder builder, string path, bool isOptional = false)
    {
        builder.Add(new YamlConfigurationSource(path, isOptional));
    }
}