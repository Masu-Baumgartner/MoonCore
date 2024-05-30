using System.Reflection;
using MoonCore.Helpers;

namespace MoonCoreUI.Helpers;

public static class FormHelper
{
    public static float CalculateMatchScore(string input, string search, char[]? normalizeChars = null)
    {
        var charsToNormalize = normalizeChars ?? new[] { ' ', '-', ':' };

        var cleanedInput = Formatter.ReplaceChars(input.ToLower(), charsToNormalize);
        var cleanedSearch = Formatter.ReplaceChars(search.ToLower(), charsToNormalize);

        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(search))
            return 0;

        // Early exit if the strings are identical
        if (cleanedInput == cleanedSearch)
            return 10000;

        float matches = 0;
        var searchLength = cleanedSearch.Length;
        var inputLength = cleanedInput.Length;

        for (var i = 0; i <= inputLength - searchLength; i++)
        {
            int j;
            for (j = 0; j < searchLength; j++)
            {
                if (cleanedInput[i + j] != cleanedSearch[j])
                    break;
            }

            if (j == searchLength)
                matches++;
        }

        // Calculate the match score based on the number of matches and the length of the search term
        var matchScore = matches / searchLength;

        return matchScore;
    }

    public static string GetPropertyValueByExpression(object? data, string expression)
    {
        if (data == null)
            return "null";

        // Simple expression
        var property = data
            .GetType()
            .GetProperties()
            .FirstOrDefault(y => y.Name == expression);

        if (property == null)
            return $"No property '{expression}' found";

        return GetPropertyValueAsString(data, property);
    }

    public static string GetPropertyValueAsString(object data, PropertyInfo property)
    {
        var propertyValue = property.GetValue(data);

        if (propertyValue == null)
            return "null";

        if (propertyValue is string propertyValueAsString)
            return propertyValueAsString;

        return propertyValue.ToString() ?? "null";
    }

    public static Func<T, string> GetPropertyExpression<T>(string expression)
    {
        return x => GetPropertyValueByExpression(x, expression);
    }

    public static bool IsGenericVersionOf(Type source, Type compare)
    {
        if (!source.IsGenericType)
            return false;

        var typeDefinition = source.GetGenericTypeDefinition();

        return typeDefinition == compare;
    }
}