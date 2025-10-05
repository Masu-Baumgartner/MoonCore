using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonCore.Common;

/// <summary>
/// Factory to create a suitable converter for the generic 
/// </summary>
public class CountedDataConverterFactory : JsonConverterFactory
{
    /// <inhertitdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;
        
        return typeToConvert.GetGenericTypeDefinition() == typeof(CountedData<>);
    }

    /// <inhertitdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        
        var converterType = typeof(CountedDataConverter<>).MakeGenericType(elementType);
        
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}