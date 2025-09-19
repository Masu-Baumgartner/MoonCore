using Microsoft.Extensions.Configuration;

namespace MoonCore.EnvConfiguration;

public class EnvConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// <b>Optional:</b> Prefix of the environment variables. Defaults to an empty string
    /// </summary>
    public string? Prefix { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Separator of the configuration sections. Defaults to <b>_</b>
    /// </summary>
    public string? Separator { get; set; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EnvConfigurationProvider(Prefix, Separator);
    }
}