using MoonCore.Exceptions;

namespace MoonCore.Helpers;

public class ChrootFileSystem
{
    private readonly string RootDirectory;

    public ChrootFileSystem(string rootDirectory)
    {
        RootDirectory = rootDirectory.EndsWith("/") ? rootDirectory : rootDirectory + "/";
    }

    public FileInfo Stat(string path)
    {
        var parentDirectory = StatParentDirectory(path);
        var fileName = Path.GetFileName(path);
        
        var fileInfo = parentDirectory
            .GetFiles()
            .FirstOrDefault(x => x.Name == fileName);

        if (fileInfo == null)
            throw new FileNotFoundException(path);
                
        if(fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) || fileInfo.LinkTarget != null)
            throw new UnsafeAccessException($"Unsafe access detected: {path}");

        return fileInfo;
    }
    
    public DirectoryInfo StatDirectory(string path)
    {
        if (path.Contains(".."))
            throw new UnsafeAccessException("Path transversal detected");
        
        if (path == "/")
            return new DirectoryInfo(RootDirectory);
        
        var parts = path
            .Split("/")
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        DirectoryInfo? info = null;

        for (int i = 0; i <= parts.Length; i++)
        {
            if (info == null)
                info = new DirectoryInfo(RootDirectory + parts[i]);
            else
            {
                info = info
                    .GetDirectories()
                    .FirstOrDefault(x => x.Name == parts[i]);
                
                if(info == null)
                    throw new DirectoryNotFoundException(path);
            }
            
            if (info != null && (info.Attributes.HasFlag(FileAttributes.ReparsePoint) || info.LinkTarget != null))
                throw new UnsafeAccessException($"Unsafe access detected: {path}");

            if (info != null && i + 1 == parts.Length)
                return info;
        }
        
        throw new DirectoryNotFoundException(path);
    }

    public DirectoryInfo StatParentDirectory(string path)
    {
        var nameOfDir = Path.GetFileName(path);
        var pathOfTheParentDir = Formatter.ReplaceEnd(path, nameOfDir, "");

        return StatDirectory(pathOfTheParentDir);
    }

    public Task EnsureDirectory(string path)
    {
        if (path.Contains(".."))
            throw new UnsafeAccessException("Path transversal detected");

        if (path == "/")
            return Task.CompletedTask;
        
        var parts = path
            .Split("/")
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        DirectoryInfo? info = null;

        foreach (var part in parts)
        {
            if (info == null)
                info = new DirectoryInfo(RootDirectory + part);
            else
            {
                var directory = info
                    .GetDirectories()
                    .FirstOrDefault(x => x.Name == part);

                if (directory == null)
                    info = info.CreateSubdirectory(part);
                else
                    info = directory;
            }
            
            if (info != null && (info.Attributes.HasFlag(FileAttributes.ReparsePoint) || info.LinkTarget != null))
                throw new UnsafeAccessException($"Unsafe access detected: {path}");
        }
        
        return Task.CompletedTask;
    }
    
    public async Task EnsureParentDirectory(string path)
    {
        var nameOfDir = Path.GetFileName(path);
        var pathOfTheParentDir = Formatter.ReplaceEnd(path, nameOfDir, "");

        await EnsureDirectory(pathOfTheParentDir);
    }

    public FileInfo[] ListFiles(string path)
    {
        var directory = StatDirectory(path);
        
        return directory.GetFiles();
    }

    public DirectoryInfo[] ListDirectories(string path)
    {
        var directory = StatDirectory(path);

        return directory.GetDirectories();
    }

    public Task<Stream> ReadFileStream(string path)
    {
        var file = Stat(path);

        var fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        return Task.FromResult<Stream>(fs);
    }

    public async Task<string> ReadFile(string path)
    {
        await using var fs = await ReadFileStream(path);
        using var sr = new StreamReader(fs);

        return await sr.ReadToEndAsync();
    }

    public async Task<Stream> OpenFileWriteStream(string path)
    {
        FileInfo file;

        try
        {
            file = Stat(path);
        }
        catch (FileNotFoundException)
        {
            try
            {
                var parentDirectory = StatParentDirectory(path);

                var filePath = parentDirectory.FullName + "/" + Path.GetFileName(path);
                file = new FileInfo(filePath);
            }
            catch (DirectoryNotFoundException)
            {
                await EnsureDirectory(path);
                
                var parentDirectory = StatParentDirectory(path);

                var filePath = parentDirectory.FullName + "/" + Path.GetFileName(path);
                file = new FileInfo(filePath);
            }
        }

        FileStream fs;

        if (file.Exists)
            fs = file.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
        else
            fs = file.Create();

        return fs;
    }

    public async Task WriteStreamToFile(string path, Stream stream)
    {
        await using var fs = await OpenFileWriteStream(path);
        await stream.CopyToAsync(fs);
    }

    public async Task WriteFile(string path, string text)
    {
        await using var fs = await OpenFileWriteStream(path);
        await using var sw = new StreamWriter(fs);

        await sw.WriteAsync(text);
    }

    public Task CreateDirectory(string path)
    {
        var nameOfDir = Path.GetFileName(path);
        
        var directory = StatParentDirectory(path);

        directory.CreateSubdirectory(nameOfDir);
        
        return Task.CompletedTask;
    }

    public async Task CreateFile(string path)
    {
        await WriteFile(path, string.Empty);
    }

    public Task DeleteFile(string path)
    {
        var file = Stat(path);
        
        file.Delete();
        
        return Task.CompletedTask;
    }

    public Task DeleteDirectory(string path)
    {
        var directory = StatDirectory(path);
        
        directory.Delete();
        
        return Task.CompletedTask;
    }

    public Task MoveFile(string source, string target)
    {
        var sourceFile = Stat(source);

        var parentDir = StatParentDirectory(target);
        var targetPath = parentDir.FullName + "/" + Path.GetFileName(target);
        
        sourceFile.MoveTo(targetPath, true);
        
        return Task.CompletedTask;
    }

    public Task MoveDirectory(string source, string target)
    {
        var sourceDirectory = StatDirectory(source);

        var parentDir = StatParentDirectory(target);
        var targetPath = parentDir.FullName + "/" + Path.GetFileName(target);
        
        sourceDirectory.MoveTo(targetPath);
        
        return Task.CompletedTask;
    }
}