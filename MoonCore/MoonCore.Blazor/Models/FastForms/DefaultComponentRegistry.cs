using MoonCore.Blazor.Forms.FastForms;

namespace MoonCore.Blazor.Models.FastForms;

public class DefaultComponentRegistry
{
    private static Dictionary<Type, Type> LookupDictionary = new();

    public static void Register<TProperty, TComponent>() where TComponent : FastFormBaseComponent<TProperty>
    {
        LookupDictionary[typeof(TProperty)] = typeof(TComponent);
    }

    public static Type? Get(Type propertyType)
    {
        if (LookupDictionary.TryGetValue(propertyType, out var foundType))
            return foundType;

        return null;
    }
}