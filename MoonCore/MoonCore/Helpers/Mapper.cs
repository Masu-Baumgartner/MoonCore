using System.Reflection;

namespace MoonCore.Helpers;

public static class Mapper
{
    public static T Map<T>(object data)
    {
        var type = typeof(T);
        var result = (T)Activator.CreateInstance(type)!;

        Dictionary<string, PropertyInfo> targetProps = new();
        foreach (var prop in type.GetProperties())
        {
            var formattedName = prop.Name.ToLower().Replace("_", "");
            
            targetProps[formattedName] = prop;
        }

        foreach (var dataProp in data.GetType().GetProperties())
        {
            var formattedName = dataProp.Name.ToLower().Replace("_", "");

            if (targetProps.ContainsKey(formattedName))
            {
                targetProps[formattedName].SetValue(result, dataProp.GetValue(data));
            }
        }

        return result;
    }

    public static T Map<T>(T source, object data)
    {
        var type = typeof(T);
        var result = source;

        Dictionary<string, PropertyInfo> targetProps = new();
        foreach (var prop in type.GetProperties())
        {
            var formattedName = prop.Name.ToLower().Replace("_", "");
            
            targetProps[formattedName] = prop;
        }

        foreach (var dataProp in data.GetType().GetProperties())
        {
            var formattedName = dataProp.Name.ToLower().Replace("_", "");

            if (targetProps.ContainsKey(formattedName))
            {
                targetProps[formattedName].SetValue(result, dataProp.GetValue(data));
            }
        }

        return result;
    }
}