using Microsoft.Extensions.Configuration;

namespace MoonCore.YamlConfiguration;

public class YamlConfigurationSource : IConfigurationSource
{
    private readonly string Path;
    private readonly bool IsOptional;

    public YamlConfigurationSource(string path, bool isOptional)
    {
        Path = path;
        IsOptional = isOptional;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new YamlConfigurationProvider(Path, IsOptional);
    }
}