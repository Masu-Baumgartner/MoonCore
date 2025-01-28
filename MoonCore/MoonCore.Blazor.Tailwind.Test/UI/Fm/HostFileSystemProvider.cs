using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using MoonCore.Blazor.Tailwind.Test.UI.Fm.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public class HostFileSystemProvider : IFileSystemProvider, ICompressFileSystemProvider
{
    private readonly string BaseDirectory;

    public HostFileSystemProvider(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    public async Task<FileSystemEntry[]> List(string path)
    {
        var entries = new List<FileSystemEntry>();

        var files = Directory.GetFiles(PathBuilder.Dir(BaseDirectory, path));

        foreach (var file in files)
        {
            var fi = new FileInfo(file);

            entries.Add(new FileSystemEntry()
            {
                Name = fi.Name,
                Size = fi.Length,
                CreatedAt = fi.CreationTimeUtc,
                IsFile = true,
                UpdatedAt = fi.LastWriteTimeUtc
            });
        }

        var directories = Directory.GetDirectories(PathBuilder.Dir(BaseDirectory, path));

        foreach (var directory in directories)
        {
            var di = new DirectoryInfo(directory);

            entries.Add(new FileSystemEntry()
            {
                Name = di.Name,
                Size = 0,
                CreatedAt = di.CreationTimeUtc,
                UpdatedAt = di.LastWriteTimeUtc,
                IsFile = false
            });
        }

        return entries.ToArray();
    }

    public async Task Create(string path, Stream stream)
    {
        var baseDir = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(baseDir))
            Directory.CreateDirectory(PathBuilder.Dir(BaseDirectory, baseDir));

        await using var fs = File.Create(PathBuilder.File(BaseDirectory, path));

        await stream.CopyToAsync(fs);

        await fs.FlushAsync();
        fs.Close();
        stream.Close();
    }

    public Task Move(string oldPath, string newPath)
    {
        if (Directory.Exists(PathBuilder.Dir(BaseDirectory, oldPath)))
        {
            Directory.Move(
                PathBuilder.Dir(BaseDirectory, oldPath),
                PathBuilder.Dir(BaseDirectory, newPath)
            );
        }
        else
        {
            File.Move(
                PathBuilder.File(BaseDirectory, oldPath),
                PathBuilder.File(BaseDirectory, newPath)
            );
        }

        return Task.CompletedTask;
    }

    public Task Delete(string path)
    {
        if (Directory.Exists(PathBuilder.Dir(BaseDirectory, path)))
            Directory.Delete(PathBuilder.Dir(BaseDirectory, path), true);
        else
            File.Delete(PathBuilder.File(BaseDirectory, path));

        return Task.CompletedTask;
    }

    public Task CreateDirectory(string path)
    {
        Directory.CreateDirectory(PathBuilder.Dir(BaseDirectory, path));
        return Task.CompletedTask;
    }

    public async Task<Stream> Read(string path)
    {
        var fs = File.Open(PathBuilder.File(BaseDirectory, path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return fs;
    }

    public CompressType[] CompressTypes { get; } =
    [
        new()
        {
            Extension = "zip",
            DisplayName = "ZIP Archive"
        },
        new()
        {
            Extension = "tar.gz",
            DisplayName = "GZ Compressed Tar Archive"
        }
    ];
    
    public async Task Compress(CompressType type, string path, string[] itemsToCompress)
    {
        if (type.Extension == "tar.gz")
            await CompressTarGz(path, itemsToCompress);
        else if (type.Extension == "zip")
            await CompressZip(path, itemsToCompress);
    }

    #region Tar Gz

    private async Task CompressTarGz(string path, string[] itemsToCompress)
    {
        var destination = PathBuilder.File(BaseDirectory, path);

        await using var outStream = File.Create(destination);
        await using var gzoStream = new GZipOutputStream(outStream);
        await using var tarStream = new TarOutputStream(gzoStream, Encoding.UTF8);

        foreach (var itemName in itemsToCompress)
        {
            var filePath = PathBuilder.File(BaseDirectory, itemName);
            var fi = new FileInfo(filePath);

            if (fi.Exists)
                await AddFileToTarGz(tarStream, filePath);
            else
                await AddDirectoryToTarGz(tarStream, PathBuilder.Dir(BaseDirectory, itemName));
        }

        await tarStream.FlushAsync();
        await gzoStream.FlushAsync();
        await outStream.FlushAsync();
        
        tarStream.Close();
        gzoStream.Close();
        outStream.Close();
    }

    private async Task AddDirectoryToTarGz(TarOutputStream tarOutputStream, string root)
    {
        foreach (var file in Directory.GetFiles(root))
            await AddFileToTarGz(tarOutputStream, file);

        foreach (var directory in Directory.GetDirectories(root))
            await AddDirectoryToTarGz(tarOutputStream, directory);
    }

    private async Task AddFileToTarGz(TarOutputStream tarOutputStream, string file)
    {
        // Open file stream
        var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        
        // Meta 
        var entry = TarEntry.CreateTarEntry(file);

        // Fix path
        entry.Name = Formatter
            .ReplaceStart(entry.Name, BaseDirectory, "")
            .TrimStart('/');
        
        entry.Size = fs.Length;
        
        // Write entry
        await tarOutputStream.PutNextEntryAsync(entry, CancellationToken.None);
        
        // Copy file content to tar stream
        await fs.CopyToAsync(tarOutputStream);
        fs.Close();
        
        // Close the entry
        tarOutputStream.CloseEntry();
    }

    #endregion

    #region ZIP

    private async Task CompressZip(string path, string[] itemsToCompress)
    {
        var destination = PathBuilder.File(BaseDirectory, path);
        
        await using var outStream = File.Create(destination);
        await using var zipOutputStream = new ZipOutputStream(outStream);
        
        foreach (var itemName in itemsToCompress)
        {
            var filePath = PathBuilder.File(BaseDirectory, itemName);
            var fi = new FileInfo(filePath);

            if (fi.Exists)
                await AddFileToZip(zipOutputStream, filePath);
            else
                await AddDirectoryToZip(zipOutputStream, PathBuilder.Dir(BaseDirectory, itemName));
        }

        await zipOutputStream.FlushAsync();
        await outStream.FlushAsync();
        
        zipOutputStream.Close();
        outStream.Close();
    }

    private async Task AddFileToZip(ZipOutputStream zipOutputStream, string path)
    {
        // Open file stream
        var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        
        // Fix path
        var name = Formatter
            .ReplaceStart(path, BaseDirectory, "")
            .TrimStart('/');
        
        // Meta 
        var entry = new ZipEntry(name);
        
        entry.Size = fs.Length;
        
        // Write entry
        await zipOutputStream.PutNextEntryAsync(entry, CancellationToken.None);
        
        // Copy file content to tar stream
        await fs.CopyToAsync(zipOutputStream);
        fs.Close();
        
        // Close the entry
        zipOutputStream.CloseEntry();
    }
    
    private async Task AddDirectoryToZip(ZipOutputStream zipOutputStream, string root)
    {
        foreach (var file in Directory.GetFiles(root))
            await AddFileToZip(zipOutputStream, file);

        foreach (var directory in Directory.GetDirectories(root))
            await AddDirectoryToZip(zipOutputStream, directory);
    }

    #endregion

    public async Task Decompress(CompressType type, string path, string destination)
    {
        if (type.Extension == "tar.gz")
            await DecompressTarGz(path, destination);
        /*else if (type.Extension == "zip")
            await CompressZip(path, itemsToCompress);*/
    }

    #region Tar Gz

    private async Task DecompressTarGz(string path, string destination)
    {
        var archivePath = PathBuilder.File(BaseDirectory, path);

        await using var fs = File.Open(archivePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var gzipInputStream = new GZipInputStream(fs);
        await using var tarInputStream = new TarInputStream(gzipInputStream, Encoding.UTF8);

        while (true)
        {
            var entry = await tarInputStream.GetNextEntryAsync(CancellationToken.None);
            
            if(entry == null)
                break;

            var fileDestination = PathBuilder.File(BaseDirectory, destination, entry.Name);
            var parentFolder = Path.GetDirectoryName(fileDestination);

            // Ensure parent directory exists, if it's not the base directory
            if (parentFolder != null && parentFolder != BaseDirectory)
                Directory.CreateDirectory(parentFolder);

            await using var fileDestinationFs = File.Open(fileDestination, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            await tarInputStream.CopyEntryContentsAsync(fileDestinationFs, CancellationToken.None);

            await fileDestinationFs.FlushAsync();
            fileDestinationFs.Close();
        }
        
        tarInputStream.Close();
        gzipInputStream.Close();
        fs.Close();
    }

    #endregion
}