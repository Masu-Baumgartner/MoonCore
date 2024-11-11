using Microsoft.Extensions.Primitives;

namespace MoonCore.Extensions;

public static class StringValuesExtension
{
    public static bool TryGetStringValue(this StringValues values, out string val)
    {
        var x = values.FirstOrDefault();
        val = x!;
        return x != null;
    }
    
    public static bool TryGetNotNullStringValue(this StringValues values, out string val)
    {
        if (!values.TryGetStringValue(out var strVal))
        {
            val = default!;
            return false;
        }

        if (string.IsNullOrEmpty(strVal))
        {
            val = default!;
            return false;
        }

        val = strVal;
        return true;
    }
}