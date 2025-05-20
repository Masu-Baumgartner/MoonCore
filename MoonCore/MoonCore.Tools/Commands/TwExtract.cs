using System.Text.Json;
using System.Text.RegularExpressions;
using Cocona;

namespace MoonCore.Tools.Commands;

public class TwExtract
{
    [Command("tw-extract", Aliases = ["tailwind-extract"], Description = "This utility allows you to extract all used tailwind classes from a generated css file")]
    public async Task Run(
        [Argument("css-path")] string cssPath,
        [Option("output", shortNames: ['o'])] string outputPath
    )
    {
        if (!File.Exists(cssPath))
        {
            Console.WriteLine($"The specified css file could not be found: {cssPath}");
            return;
        }
        
        var cssContent = await File.ReadAllTextAsync(cssPath);

        var cssClasses = ExtractClassNamesFromLayers(cssContent, [
            "components",
            "utilities"
        ]);

        var json = JsonSerializer.Serialize(cssClasses, new JsonSerializerOptions()
        {
            WriteIndented = true
        });

        if (!string.IsNullOrEmpty(outputPath))
        {
            await File.WriteAllTextAsync(outputPath, json);
            return;
        }
        
        Console.Write(json);
    }
    
    private string[] ExtractClassNamesFromLayers(string css, string[] targetLayers)
    {
        var result = new List<string>();

        foreach (var layer in targetLayers)
        {
            var pattern = @$"@layer\s+{layer}\s*\{{";
            var matches = Regex.Matches(css, pattern);

            foreach (Match match in matches)
            {
                var startIndex = match.Index + match.Length;
                var endIndex = FindMatchingBrace(css, startIndex - 1);
                if (endIndex == -1) continue;

                var layerContent = css.Substring(startIndex, endIndex - startIndex);

                // Updated regex to match escaped class names like .lg\:ring-white\/10
                var classMatches = Regex.Matches(layerContent, @"\.([^\s,{]+)");
                
                foreach (Match classMatch in classMatches)
                {
                    var className = classMatch.Groups[1].Value;

                    className = className.Replace("\\:", ":");
                    className = className.Replace("\\[", "[");
                    className = className.Replace("\\]", "]");
                    className = className.Replace("\\/", "/");
                    className = className.Replace("\\.", ".");
                
                    result.Add(className);
                }
            }
        }

        return result.ToArray();
    }

    private int FindMatchingBrace(string input, int startIndex)
    {
        var depth = 0;
        
        for (var i = startIndex; i < input.Length; i++)
        {
            if (input[i] == '{') depth++;
            else if (input[i] == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        
        return -1;
    }
}