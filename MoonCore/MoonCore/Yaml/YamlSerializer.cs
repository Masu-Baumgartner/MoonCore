using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MoonCore.Yaml;

public static class YamlSerializer
{
    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithIndentedSequences()
        .WithQuotingNecessaryStrings()
        .Build();

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public static string Serialize<T>(T item)
        => Serializer.Serialize(item);

    public static T Deserialize<T>(string yaml)
        => Deserializer.Deserialize<T>(yaml);
}