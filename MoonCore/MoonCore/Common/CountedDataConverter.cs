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
     
        var namingPolicy = options.PropertyNamingPolicy;
        var totalCountName = namingPolicy?.ConvertName("TotalCount") ?? "TotalCount";
        var itemsName = namingPolicy?.ConvertName("Items") ?? "Items";
        
        var totalCount = root.GetProperty(totalCountName).GetInt32();
        var items = root.GetProperty(itemsName).Deserialize<IEnumerable<T>>(options)!;
        
        return new CountedData<T>(items, totalCount);
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
}