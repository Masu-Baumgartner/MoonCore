using MoonCore.Blazor.Forms.Fast.Components;

namespace MoonCore.Blazor.Models.Fast;

public class DefaultComponentSelector
{
    private static Dictionary<Type, Type> LookupDictionary = new();

    public static void RegisterDefault<TProperty, TComponent>() where TComponent : BaseFastFormComponent<TProperty>
    {
        LookupDictionary[typeof(TProperty)] = typeof(TComponent);
    }

    public static Type? GetDefault(Type propertyType)
    {
        if (LookupDictionary.TryGetValue(propertyType, out var foundType))
            return foundType;

        return null;
    }
}