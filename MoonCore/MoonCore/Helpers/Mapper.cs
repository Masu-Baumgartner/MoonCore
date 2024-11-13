using System.Reflection;
using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers;

public static class Mapper
{
    public static object MapRaw(object source, object data, bool ignoreNullValues = false, ILogger? logger = null)
    {
        var sourceProperties = new Dictionary<string, PropertyInfo>();

        foreach (var property in source.GetType().GetProperties())
            sourceProperties[BuildId(property.Name)] = property;

        var dataProperties = new Dictionary<string, PropertyInfo>();

        foreach (var property in data.GetType().GetProperties())
            dataProperties[BuildId(property.Name)] = property;

        foreach (var dataProperty in dataProperties)
        {
            try
            {
                if (!sourceProperties.TryGetValue(dataProperty.Key, out var sourceProperty))
                    continue;

                var value = dataProperty.Value.GetValue(data);

                if (ignoreNullValues && value == null)
                    continue;

                sourceProperty.SetValue(source, value);
            }
            catch (Exception e)
            {
                logger?.LogWarning("Unable to map property {name}: {e}", dataProperty.Key, e);
            }
        }

        return source;
    }

    #region Object creating mappers

    public static object Map(Type type, object data, bool ignoreNullValues = false, ILogger? logger = null) => MapRaw(Activator.CreateInstance(type)!, data, ignoreNullValues, logger);

    public static T Map<T>(object data, bool ignoreNullValues = false, ILogger? logger = null) => (T)Map(typeof(T), data, ignoreNullValues, logger);

    #endregion

    public static T Map<T>(T source, object data, bool ignoreNullValues = false, ILogger? logger = null) => (T)MapRaw(source!, data, ignoreNullValues, logger);

    private static string BuildId(string name) => name
        .Replace(".", "")
        .Replace("-", "")
        .Replace("_", "")
        .ToLower();
}