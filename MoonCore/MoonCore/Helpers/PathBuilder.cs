namespace MoonCore.Helpers;

public static class PathBuilder
{
    /// <summary>
    /// Builds a cross platform path to a directory
    /// </summary>
    /// <param name="parts">The parts of the path</param>
    /// <returns></returns>
    public static string Dir(params string[] parts)
    {
        var res = "";

        foreach (var part in parts)
        {
            res += part + Path.DirectorySeparatorChar;
        }

        return res.Replace(
            $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", 
            $"{Path.DirectorySeparatorChar}"
        );
    }
    
    /// <summary>
    /// Builds a cross platform path to a file
    /// </summary>
    /// <param name="parts">The parts of the path</param>
    /// <returns></returns>
    public static string File(params string[] parts)
    {
        var res = "";

        foreach (var part in parts)
        {
            res += part + (part == parts.Last() ? "" : Path.DirectorySeparatorChar);
        }

        return res.Replace(
            $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", 
            $"{Path.DirectorySeparatorChar}"
        );
    }
    
    public static string FullPathUnix(string currentPath, string diff)
    {
        // Handle special cases for currentPath or diff
        if (string.IsNullOrEmpty(currentPath))
            throw new ArgumentException("CurrentPath cannot be null or empty.");

        if (!currentPath.StartsWith("/"))
            throw new ArgumentException("CurrentPath must be an absolute path starting with '/'.");

        // Split the paths into components
        var pathComponents = currentPath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        
        var diffComponents = diff
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        // Process each part of the diff
        foreach (var part in diffComponents)
        {
            if (part == "..")
            {
                // Move up one directory
                if (pathComponents.Count > 0)
                    pathComponents.RemoveAt(pathComponents.Count - 1);
            }
            else if (part != ".")
            {
                // Add the part to the path
                pathComponents.Add(part);
            }
        }

        // Build the final path
        var finalPath = "/" + string.Join("/", pathComponents);
        return finalPath;
    }
    
    public static string JoinPaths(params string[] parts)
    {
        if (parts == null || parts.Length == 0)
            throw new ArgumentException("At least one path part must be provided.");

        var result = string.Empty;

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            if (result.EndsWith('/') && part.StartsWith('/'))
            {
                // Avoid double slashes by removing the leading slash of the current part
                result += part.Substring(1);
            }
            else if (!result.EndsWith('/') && !part.StartsWith('/'))
            {
                // Add a single slash between parts
                result += "/" + part;
            }
            else
            {
                // Directly concatenate otherwise
                result += part;
            }
        }

        // Ensure the resulting path doesn't end with a trailing slash, unless it's the root
        if (result.Length > 1 && result.EndsWith('/'))
            result = result.Substring(0, result.Length - 1);

        return result;
    }
}