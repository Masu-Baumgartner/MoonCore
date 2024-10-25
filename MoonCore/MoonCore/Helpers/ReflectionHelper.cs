using System.Reflection;

namespace MoonCore.Helpers;

public static class ReflectionHelper
{
    public static bool IsGenericVersionOf(Type source, Type compare)
    {
        if (!source.IsGenericType)
            return false;

        var typeDefinition = source.GetGenericTypeDefinition();

        return typeDefinition == compare;
    }

    public static T Cast<T>(object input)
        => (T)input;

    public static object CastWithType(object input, Type type)
    {
        return typeof(ReflectionHelper)
            .GetMethod("Cast")!
            .MakeGenericMethod(type)
            .Invoke(null, [input])!;
    }

    public static MethodInfo? GetStaticMethod(Type type, string name)
    {
        return type.GetMethod(
            name,
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
    }

    public static object? InvokeGenericMethod(
        object? instance,
        Type type,
        string name,
        Type[] genericTypes,
        object[] parameters
        )
    {
        MethodInfo? method;

        if (instance == null)
            method = GetStaticMethod(type, name);
        else
            method = type.GetMethod(name);

        if (method == null)
            throw new ArgumentException($"Unable to find a method with the name '{name}'");

        var genericMethod = method.MakeGenericMethod(genericTypes);

        return genericMethod.Invoke(instance, parameters);
    }
}