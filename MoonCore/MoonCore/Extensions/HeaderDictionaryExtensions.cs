using Microsoft.AspNetCore.Http;

namespace MoonCore.Extensions;

public static class HeaderDictionaryExtensions
{
    public static bool TryGetNotNull(this IHeaderDictionary dic, string key, out string val)
    {
        if (!dic.TryGetValue(key, out var strVal))
        {
            val = default!;
            return false;
        }

        return strVal.TryGetNotNullStringValue(out val);
    }
}