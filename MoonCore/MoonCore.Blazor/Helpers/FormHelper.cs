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
}