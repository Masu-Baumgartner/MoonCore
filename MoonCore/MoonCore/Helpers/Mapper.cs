namespace MoonCore.Helpers;

public static class Mapper
{
    public static T Map<T>(object data)
    {
        var type = typeof(T);
        var result = (T)Activator.CreateInstance(type)!;
        
        return Map<T>(result, data);
    }

    public static T Map<T>(T source, object data)
    {
        var type = typeof(T);
        var result = source;

        var targetProps = type
            .GetProperties()
            .ToDictionary(
                x => x.Name
                    .ToLower()
                    .Replace("_", ""),
                x => x
            );

        foreach (var prop in type.GetProperties())
        {
            var formattedName = prop.Name
                .ToLower()
                .Replace("_", "");

            targetProps.Add(formattedName, prop);
        }

        foreach (var dataProp in data.GetType().GetProperties())
        {
            var formattedName = dataProp.Name
                .ToLower()
                .Replace("_", "");

            if (targetProps.ContainsKey(formattedName))
                targetProps[formattedName].SetValue(result, dataProp.GetValue(data));
        }

        return result;
    }
}