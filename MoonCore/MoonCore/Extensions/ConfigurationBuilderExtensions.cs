using System.Text;
using Microsoft.Extensions.Configuration;

namespace MoonCore.Extensions;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// An extension method to add a string directory to a configuration
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddJsonString(this IConfigurationBuilder configurationBuilder, string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        using var stream = new MemoryStream(bytes);
        return configurationBuilder.AddJsonStream(stream);
    }
}