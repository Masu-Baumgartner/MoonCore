using System.Collections;
using Microsoft.Extensions.Configuration;

namespace MoonCore.EnvConfiguration;

public class EnvConfigurationProvider : ConfigurationProvider
{
    private readonly string Prefix;
    private readonly string NormalizedPrefix;
    private readonly string Separator;
    
    public EnvConfigurationProvider()
    {
        Separator = "__";
        Prefix = string.Empty;
        NormalizedPrefix = string.Empty;
    }

    public EnvConfigurationProvider(string? prefix, string? separator)
    {
        Separator = separator ?? "__";
        Prefix = prefix ?? string.Empty;
        NormalizedPrefix = Normalize(Prefix);
    }
    
    public override void Load() =>
        Load(Environment.GetEnvironmentVariables());
    
    public override string ToString()
    {
        var s = GetType().Name;
        
        if (!string.IsNullOrEmpty(Prefix))
            s += $" Prefix: '{Prefix}'";

        return s;
    }

    private void Load(IDictionary envVariables)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var e = envVariables.GetEnumerator();
        
        try
        {
            while (e.MoveNext())
            {
                var key = (string)e.Entry.Key;
                var value = (string?)e.Entry.Value;

                AddIfNormalizedKeyMatchesPrefix(data, Normalize(key), value);
            }
        }
        finally
        {
            (e as IDisposable)?.Dispose();
        }

        Data = data;
    }

    private void AddIfNormalizedKeyMatchesPrefix(Dictionary<string, string?> data, string normalizedKey, string? value)
    {
        if (normalizedKey.StartsWith(NormalizedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            data[normalizedKey.Substring(NormalizedPrefix.Length)] = value;
        }
    }

    private string Normalize(string key) => key.Replace(Separator, ConfigurationPath.KeyDelimiter);
}