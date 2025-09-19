using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using YamlDotNet.RepresentationModel;

namespace MoonCore.Yaml;

/// <summary>
/// Configuration provider for yaml files
/// </summary>
public class YamlConfigurationProvider : IConfigurationProvider
{
    private readonly string Path;
    private readonly bool IsOptional;
    private readonly string? Prefix;
    private IDictionary<string, string> Data = new Dictionary<string, string>();

    public YamlConfigurationProvider(string path, bool isOptional, string? prefix = null)
    {
        Path = path;
        IsOptional = isOptional;
        Prefix = prefix;
    }

    public bool TryGet(string key, out string? value)
    {
        return Data.TryGetValue(key, out value);
    }

    public void Set(string key, string? value)
    {
        Data[key] = value ?? "";
    }

    public IChangeToken GetReloadToken()
    {
        return NullChangeToken.Singleton;
    }

    public void Load()
    {
        if (!File.Exists(Path))
        {
            if (IsOptional)
                return;

            throw new FileNotFoundException("Specified configuration file not found and not marked as optional", Path);
        }

        using var fs = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);

        var yamlStream = new YamlStream();
        yamlStream.Load(sr);

        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (yamlStream.Documents.Count == 0)
            return;

        if (yamlStream.Documents[0].RootNode is not YamlMappingNode rootMapping)
            return;

        ProcessNode(rootMapping, Prefix, data);

        Data = data;
    }

    private void ProcessNode(YamlMappingNode node, string? parentPath, IDictionary<string, string> data)
    {
        foreach (var entry in node.Children)
        {
            var key = ((YamlScalarNode)entry.Key).Value ?? "";
            var fullKey = parentPath == null ? key : $"{parentPath}:{key}";

            switch (entry.Value)
            {
                case YamlScalarNode scalar:
                    data[fullKey] = scalar.Value ?? "";
                    break;

                case YamlMappingNode mapping:
                    ProcessNode(mapping, fullKey, data);
                    break;

                case YamlSequenceNode sequence:
                    var index = 0;
                    foreach (var item in sequence.Children)
                    {
                        var itemKey = $"{fullKey}:{index}";
                        
                        switch (item)
                        {
                            case YamlScalarNode scalarItem:
                                data[itemKey] = scalarItem.Value ?? "";
                                break;
                            case YamlMappingNode mappingItem:
                                ProcessNode(mappingItem, itemKey, data);
                                break;
                        }

                        index++;
                    }

                    break;
            }
        }
    }

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        var prefix = parentPath == null ? "" : parentPath + ":";
        
        return Data
            .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Key.Substring(prefix.Length))
            .Select(segment => segment.Split(':')[0])
            .Concat(earlierKeys)
            .Distinct()
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
    }
}