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
}