using System.Linq.Expressions;
using System.Reflection;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Helpers;

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

    public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object?>> expression)
    {
        MemberExpression? memberExpression = default;

        if (expression.Body is MemberExpression body)
            memberExpression = body;
        else if (expression.Body is UnaryExpression unaryExpression &&
                 unaryExpression.NodeType == ExpressionType.Convert)
        {
            if (unaryExpression.Operand is MemberExpression operand)
                memberExpression = operand;
        }

        if (memberExpression == null)
            throw new ArgumentException("Unable to process expression");

        if (memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }

        throw new ArgumentException("Expression does not represent a property.");
    }
    
    public static PropertyInfo GetPropertyInfo<TModel, TProperty>(Expression<Func<TModel, TProperty?>> expression)
    {
        MemberExpression? memberExpression = default;

        if (expression.Body is MemberExpression body)
            memberExpression = body;
        else if (expression.Body is UnaryExpression unaryExpression &&
                 unaryExpression.NodeType == ExpressionType.Convert)
        {
            if (unaryExpression.Operand is MemberExpression operand)
                memberExpression = operand;
        }

        if (memberExpression == null)
            throw new ArgumentException("Unable to process expression");

        if (memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }

        throw new ArgumentException("Expression does not represent a property.");
    }
}