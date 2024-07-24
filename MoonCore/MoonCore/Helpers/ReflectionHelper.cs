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
}