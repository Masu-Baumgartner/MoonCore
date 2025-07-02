using Microsoft.Extensions.Configuration;

namespace MoonCore.Yaml;

public class YamlConfigurationSource : IConfigurationSource
{
    private readonly string Path;
    private readonly bool IsOptional;
    private readonly string? Prefix;

    public YamlConfigurationSource(string path, bool isOptional, string? prefix = null)
    {
        Path = path;
        IsOptional = isOptional;
        Prefix = prefix;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new YamlConfigurationProvider(Path, IsOptional, Prefix);
    }
}