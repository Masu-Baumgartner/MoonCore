namespace MoonCore.Helpers;

public static class UnixPath
{
    public static readonly char DirectorySeparatorChar = '/';

    public static string Combine(params string[] paths)
    {
        return string.Join(DirectorySeparatorChar, paths.Select(p => p.Trim(DirectorySeparatorChar)));
    }

    public static string GetFileName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        
        var parts = path.Split(DirectorySeparatorChar);
        
        return parts[^1];
    }

    public static string GetFileNameWithoutExtension(string path)
    {
        var fileName = GetFileName(path);
        var index = fileName.LastIndexOf('.');
        
        return index >= 0 ? fileName.Substring(0, index) : fileName;
    }

    public static string GetExtension(string path)
    {
        var fileName = GetFileName(path);
        var index = fileName.LastIndexOf('.');
        
        return index >= 0 ? fileName.Substring(index) : string.Empty;
    }

    public static string GetDirectoryName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        
        var index = path.LastIndexOf(DirectorySeparatorChar);
        
        return index > 0 ? path.Substring(0, index) : string.Empty;
    }

    public static string ChangeExtension(string path, string newExtension)
    {
        var dir = GetDirectoryName(path);
        
        var fileNameWithoutExt = GetFileNameWithoutExtension(path);
        
        if (!string.IsNullOrEmpty(newExtension) && !newExtension.StartsWith("."))
            newExtension = "." + newExtension;
        
        return Combine(dir, fileNameWithoutExt + newExtension);
    }

    public static string GetFullPath(string path, string basePath = "/")
    {
        if (IsPathRooted(path))
            return NormalizePath(path);
        
        return NormalizePath(Combine(basePath, path));
    }

    public static bool IsPathRooted(string path)
    {
        return !string.IsNullOrEmpty(path) && path[0] == DirectorySeparatorChar;
    }

    public static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;
        var segments = new Stack<string>();
        foreach (var part in path.Split(DirectorySeparatorChar))
        {
            if (part == "..")
            {
                if (segments.Count > 0) segments.Pop();
            }
            else if (!string.IsNullOrEmpty(part) && part != ".")
            {
                segments.Push(part);
            }
        }
        var normalized = string.Join(DirectorySeparatorChar, segments.Reverse());
        return IsPathRooted(path) ? DirectorySeparatorChar + normalized : normalized;
    }

    public static string GetRelativePath(string basePath, string targetPath)
    {
        var baseParts = NormalizePath(basePath).Trim(DirectorySeparatorChar).Split(DirectorySeparatorChar);
        var targetParts = NormalizePath(targetPath).Trim(DirectorySeparatorChar).Split(DirectorySeparatorChar);
        var commonLength = 0;
        while (commonLength < baseParts.Length && commonLength < targetParts.Length &&
               baseParts[commonLength] == targetParts[commonLength])
        {
            commonLength++;
        }

        var upMoves = Enumerable.Repeat("..", baseParts.Length - commonLength);
        var downMoves = targetParts.Skip(commonLength);
        return string.Join(DirectorySeparatorChar, upMoves.Concat(downMoves));
    }
}