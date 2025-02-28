using Microsoft.Extensions.Configuration;

namespace MoonCore.EnvConfiguration;

public class EnvConfigurationSource : IConfigurationSource
{
    public string? Prefix { get; set; }
    public string? Separator { get; set; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EnvConfigurationProvider(Prefix, Separator);
    }
}