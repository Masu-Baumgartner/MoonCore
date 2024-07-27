using System.Reflection;

namespace MoonCore.Helpers;

public static class Mapper
{
    public static object MapRaw(object source, object data, bool ignoreNullValues = false)
    {
        var sourceProperties = new Dictionary<string, PropertyInfo>();

        foreach (var property in source.GetType().GetProperties())
            sourceProperties[BuildId(property.Name)] = property;

        var dataProperties = new Dictionary<string, PropertyInfo>();

        foreach (var property in data.GetType().GetProperties())
            dataProperties[BuildId(property.Name)] = property;

        foreach (var dataProperty in dataProperties)
        {
            if(!sourceProperties.TryGetValue(dataProperty.Key, out var sourceProperty))
                continue;
            
            var value = dataProperty.Value.GetValue(data);

            if (ignoreNullValues && value == null)
                continue;

            sourceProperty.SetValue(source, value);
        }

        return source;
    }

    #region Object creating mappers

    public static object Map(Type type, object data, bool ignoreNullValues = false) => MapRaw(Activator.CreateInstance(type)!, data, ignoreNullValues: ignoreNullValues);

    public static T Map<T>(object data, bool ignoreNullValues = false) => (T)Map(typeof(T), data, ignoreNullValues: ignoreNullValues);

    #endregion

    public static T Map<T>(T source, object data, bool ignoreNullValues = false) => (T)MapRaw(source!, data, ignoreNullValues: ignoreNullValues);

    private static string BuildId(string name) => name
        .Replace(".", "")
        .Replace("-", "")
        .Replace("_", "")
        .ToLower();
}