using System.Text;
using Microsoft.AspNetCore.Components;

namespace MoonCore.Helpers;

/// <summary>
/// This class provides a lot of useful methods to format or generate strings
/// </summary>
public static class Formatter
{
    /// <summary>
    /// Generate a random string with all chars from A-Z, a-z, 0-9
    /// </summary>
    /// <param name="length">The length of the string</param>
    /// <returns></returns>
    public static string GenerateString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder();
        var random = new Random();

        for (var i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// This method creates a string with a fixed length from a number. So a 345 can be converted to a "00345"
    /// </summary>
    /// <param name="number">The number you want to have leading zeros in front of</param>
    /// <param name="n">The length of the string</param>
    /// <returns></returns>
    public static string IntToStringWithLeadingZeros(int number, int n)
    {
        var result = number.ToString();
        var length = result.Length;

        for (var i = length; i < n; i++)
        {
            result = "0" + result;
        }

        return result;
    }

    /// <summary>
    /// This method capitalizes the first character in a string
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns></returns>
    public static string CapitalizeFirstCharacter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var firstChar = char.ToUpper(input[0]);
        var restOfString = input.Substring(1);

        return firstChar + restOfString;
    }

    /// <summary>
    /// This method cuts a string in half
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns></returns>
    public static string CutInHalf(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var length = input.Length;
        var halfLength = length / 2;

        return input.Substring(0, halfLength);
    }

    /// <summary>
    /// Like .EndsWith but with multiple strings to check where only one needs to match in order to get a true value
    /// </summary>
    /// <param name="input">The string to check on</param>
    /// <param name="strings">The strings which the input may end with</param>
    /// <returns></returns>
    public static bool EndsInOneOf(string input, IEnumerable<string> strings)
    {
        foreach (var str in strings)
        {
            if (input.EndsWith(str))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Like .Contains but with multiple strings to check where only one needs to match in order to get a true value
    /// </summary>
    /// <param name="input">The string to check on</param>
    /// <param name="strings">The strings which may be in the input</param>
    /// <param name="foundText">The string which is found by the function</param>
    /// <returns></returns>
    public static bool ContainsOneOf(string input, IEnumerable<string> strings, out string foundText)
    {
        foreach (var str in strings)
        {
            if (input.Contains(str))
            {
                foundText = str;
                return true;
            }
        }

        foundText = "";
        return false;
    }

    /// <summary>
    /// Like .Contains but with multiple strings to check where only one needs to match in order to get a true value
    /// </summary>
    /// <param name="input">The string to check on</param>
    /// <param name="strings">The strings which may be in the input</param>
    /// <returns></returns>
    public static bool ContainsOneOf(string input, IEnumerable<string> strings)
    {
        return ContainsOneOf(input, strings, out _);
    }

    /// <summary>
    /// This functions takes a byte size and converts it to a human readable size format up to gigabytes
    /// </summary>
    /// <param name="bytes">The input bytes value</param>
    /// <param name="conversionStep">Its 1024, not 1000. If you are a 1000 user you should set conversionStep to 1000</param>
    /// <returns></returns>
    public static string FormatSize(ulong bytes, double conversionStep = 1024)
    {
        var i = Math.Abs((double)bytes) / conversionStep;

        if (i < 1)
        {
            return bytes + " B";
        }

        if (i / conversionStep < 1)
        {
            return i.Round(2) + " KB";
        }

        if (i / (conversionStep * conversionStep) < 1)
        {
            return (i / conversionStep).Round(2) + " MB";
        }

        return (i / (conversionStep * conversionStep)).Round(2) + " GB";
    }

    private static double Round(this double d, int decimals)
    {
        return Math.Round(d, decimals);
    }

    /// <summary>
    /// Like .Replace but it only replaces at the end of a string
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="substringToReplace">The string to search for</param>
    /// <param name="newSubstring">The string to replace with</param>
    /// <returns></returns>
    public static string ReplaceEnd(string input, string substringToReplace, string newSubstring)
    {
        var lastIndexOfSubstring = input.LastIndexOf(substringToReplace, StringComparison.Ordinal);
        if (lastIndexOfSubstring >= 0)
        {
            input = input.Remove(lastIndexOfSubstring, substringToReplace.Length)
                .Insert(lastIndexOfSubstring, newSubstring);
        }

        return input;
    }

    /// <summary>
    /// Like .Replace but it only replaces at the start of a string
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="substringToReplace">The string to search for</param>
    /// <param name="newSubstring">The string to replace with</param>
    /// <returns></returns>
    public static string ReplaceStart(string input, string substringToReplace, string newSubstring)
    {
        if (input.StartsWith(substringToReplace))
            return newSubstring + input.Substring(substringToReplace.Length);
        else
            return input;
    }

    /// <summary>
    /// Converts a string like "MoonCore" to "Moon Core"
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns></returns>
    public static string ConvertCamelCaseToSpaces(string input)
    {
        var output = new StringBuilder();

        foreach (var c in input)
        {
            if (char.IsUpper(c))
            {
                output.Append(' ');
            }

            output.Append(c);
        }

        return output.ToString().Trim();
    }

    /// <summary>
    /// Formats a time in milliseconds to a human readable format
    /// </summary>
    /// <param name="uptime">The time in milliseconds</param>
    /// <returns></returns>
    public static string FormatUptime(double uptime)
    {
        var t = TimeSpan.FromMilliseconds(uptime);

        return FormatUptime(t);
    }

    /// <summary>
    /// Formats a timespan to a human readable format
    /// </summary>
    /// <param name="input">The input timespan</param>
    /// <returns></returns>
    public static string FormatUptime(TimeSpan input)
    {
        if (input.Days > 0)
        {
            return $"{input.Days}d  {input.Hours}h {input.Minutes}m {input.Seconds}s";
        }
        else
        {
            return $"{input.Hours}h {input.Minutes}m {input.Seconds}s";
        }
    }

    /// <summary>
    /// Formats a datetime into the standard (NOT AMERICAN) format 
    /// </summary>
    /// <param name="input">The input datetime</param>
    /// <returns></returns>
    public static string FormatDate(DateTime input)
    {
        string i2s(int i)
        {
            if (i.ToString().Length < 2)
                return "0" + i;
            return i.ToString();
        }

        return $"{i2s(input.Day)}.{i2s(input.Month)}.{input.Year} {i2s(input.Hour)}:{i2s(input.Minute)}";
    }

    /// <summary>
    /// Formats a date into the standard (NOT AMERICAN) format 
    /// </summary>
    /// <param name="input">The input date</param>
    /// <returns></returns>
    public static string FormatDateOnly(DateTime input)
    {
        string i2s(int i)
        {
            if (i.ToString().Length < 2)
                return "0" + i;
            return i.ToString();
        }

        return $"{i2s(input.Day)}.{i2s(input.Month)}.{input.Year}";
    }

    /// <summary>
    /// This functions takes a byte size and converts it to a human readable size format up to gigabytes
    /// </summary>
    /// <param name="bytes">The input bytes value</param>
    /// <param name="conversionStep">Its 1024, not 1000. If you are a 1000 user you should set conversionStep to 1000</param>
    /// <returns></returns>
    public static string FormatSize(double bytes, double conversionStep = 1024) =>
        FormatSize((ulong)bytes, conversionStep);

    public static string FormatSize(long bytes, double conversionStep = 1024) =>
        FormatSize((ulong)bytes, conversionStep);

    public static string FormatSize(int bytes, double conversionStep = 1024) =>
        FormatSize((ulong)bytes, conversionStep);

    /// <summary>
    /// Formats a datetime to a "x days ago" format
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FormatAgoFromDateTime(DateTime input)
    {
        var timeSince = DateTime.UtcNow.Subtract(input);

        if (timeSince.TotalMilliseconds < 1)
            return "just now";

        if (timeSince.TotalMinutes < 1)
            return "less than a minute ago";

        if (timeSince.TotalMinutes < 2)
            return "1 minute ago";

        if (timeSince.TotalMinutes < 60)
            return Math.Round(timeSince.TotalMinutes) + " minutes ago";

        if (timeSince.TotalHours < 2)
            return "1 hour ago";

        if (timeSince.TotalHours < 24)
            return Math.Round(timeSince.TotalHours) + " hours ago";

        if (timeSince.TotalDays < 2)
            return "1 day ago";

        return Math.Round(timeSince.TotalDays) + " days ago";
    }

    /// <summary>
    /// This will replace every placeholder with the respective value if specified in the model
    /// For example:
    /// A instance of the user model has been passed in the 'models' parameter of the function.
    /// So the placeholder {{User.Email}} will be replaced by the value of the Email property of the model
    /// </summary>
    /// <param name="text">The input string</param>
    /// <param name="models">The input models</param>
    /// <returns></returns>
    public static string ProcessTemplating(string text, params object[] models)
    {
        foreach (var model in models)
        {
            foreach (var property in model.GetType().GetProperties())
            {
                var value = property.GetValue(model);

                if (value == null)
                    continue;

                var placeholder = "{{" + $"{model.GetType().Name}.{property.Name}" + "}}";

                text = text.Replace(placeholder, value.ToString());
            }
        }

        return text;
    }

    /// <summary>
    /// Formats multi line texts into a render fragment for blazor
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static RenderFragment FormatLineBreaks(string content)
    {
        return builder =>
        {
            var i = 0;
            var arr = content.Split("\n");

            foreach (var line in arr)
            {
                builder.AddContent(i, line);
                if (i++ != arr.Length - 1)
                {
                    builder.AddMarkupContent(i, "<br/>");
                }
            }
        };
    }

    /// <summary>
    /// This method converts a string to its base64 representation
    /// </summary>
    /// <param name="text">The text to encode into base64</param>
    /// <returns>A base64 string</returns>
    public static string FromTextToBase64(string text)
    {
        var data = Encoding.UTF8.GetBytes(text);
        return FromByteToBase64(data);
    }

    public static string FromByteToBase64(byte[] data)
    {
        var base64 = Convert.ToBase64String(data);

        return base64
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// This method converts a base64 string to a utf 8 string
    /// </summary>
    /// <param name="base64">The base64 string to decode into a utf 8 string</param>
    /// <returns>A utf 8 string</returns>
    public static string FromBase64ToText(string base64)
    {
        var data = FromBase64ToByte(base64);
        return Encoding.UTF8.GetString(data);
    }

    public static byte[] FromBase64ToByte(string base64)
    {
        base64 = base64
            .Replace('_', '/')
            .Replace('-', '+');

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }

    public static string ReplaceChars(string input, char[] chars)
    {
        var result = input;

        foreach (var c in chars)
        {
            result = result.Replace($"{c}", "");
        }

        return result;
    }
    
    public static string ToHex(byte[] bytes, bool upperCase = false)
    {
        var result = new StringBuilder(bytes.Length*2);

        foreach (var b in bytes)
            result.Append(b.ToString(upperCase ? "X2" : "x2"));

        return result.ToString();
    }
}