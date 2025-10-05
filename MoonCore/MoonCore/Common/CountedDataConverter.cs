using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonCore.Common;

/// <summary>
/// A converter for the generic <see cref="CountedData{T}"/> type.
/// This is required as the json serializer would otherwise omit 
/// </summary>
/// <typeparam name="T">Type which is stored in the counted data</typeparam>
public class CountedDataConverter<T> : JsonConverter<CountedData<T>>
{
    /// <inhertitdoc />
    public override CountedData<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!TryGetProperty(ref root, "Items", options, out var itemsElement))
            throw new JsonException("Unable to find Items property on CountedData json input");

        if (!TryGetProperty(ref root, "TotalCount", options, out var totalCountElement))
            throw new JsonException("Unable to find TotalCount property on CountedData json input");

        var items = itemsElement.Deserialize<IEnumerable<T>>(options)!;
        var totalCount = totalCountElement.GetInt32();

        return new CountedData<T>(
            items,
            totalCount
        );
    }

    /// <inhertitdoc />
    public override void Write(Utf8JsonWriter writer, CountedData<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var namingPolicy = options.PropertyNamingPolicy;

        var totalCountName = namingPolicy?.ConvertName("TotalCount") ?? "TotalCount";
        var itemsName = namingPolicy?.ConvertName("Items") ?? "Items";

        writer.WriteNumber(totalCountName, value.TotalCount);

        writer.WritePropertyName(itemsName);
        JsonSerializer.Serialize(writer, value.Items, options);

        writer.WriteEndObject();
    }

    private static bool TryGetProperty(
        ref JsonElement element,
        string propertyName,
        JsonSerializerOptions options,
        out JsonElement foundElement
    )
    {
        var namingPolicy = options.PropertyNamingPolicy;
        propertyName = namingPolicy?.ConvertName(propertyName) ?? propertyName;

        if (element.TryGetProperty(propertyName, out foundElement))
            return true;

        if (!options.PropertyNameCaseInsensitive)
            return false;

        foreach (var property in element.EnumerateObject())
        {
            if (!string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                continue;

            foundElement = property.Value;
            return true;
        }

        return false;
    }
}