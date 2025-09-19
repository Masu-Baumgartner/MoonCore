using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MoonCore.Yaml;

/// <summary>
/// Helper class for parsing yaml with default parsing rules
/// </summary>
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

    /// <summary>
    /// Serializes an object to a yaml string 
    /// </summary>
    /// <param name="item">Object to serialize</param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns></returns>
    public static string Serialize<T>(T item)
        => Serializer.Serialize(item);

    /// <summary>
    /// Deserializes a yaml string to an object of the provided type
    /// </summary>
    /// <param name="yaml">Yaml string to deserialize</param>
    /// <typeparam name="T">Type of the object to deserialize</typeparam>
    /// <returns>Deserialized object</returns>
    public static T Deserialize<T>(string yaml)
        => Deserializer.Deserialize<T>(yaml);
}