using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using MoonCore.Blazor.Tailwind.Fm;
using MoonCore.Blazor.Tailwind.Fm.Models;
using MoonCore.Blazor.Tailwind.Services;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test;

public class HostFileSystemProvider : IFileSystemProvider, ICompressFileSystemProvider
{
    private readonly string BaseDirectory;
    private readonly DownloadService DownloadService;

    public HostFileSystemProvider(string baseDirectory, DownloadService downloadService)
    {
        BaseDirectory = baseDirectory;
        DownloadService = downloadService;
    }

    public async Task<FileSystemEntry[]> List(string path)
    {
        var entries = new List<FileSystemEntry>();

        var files = Directory.GetFiles(Path.Combine(BaseDirectory, path));

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

        var directories = Directory.GetDirectories(Path.Combine(BaseDirectory, path));

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
            Directory.CreateDirectory(Path.Combine(BaseDirectory, baseDir));

        await using var fs = File.Create(Path.Combine(BaseDirectory, path));

        await stream.CopyToAsync(fs);

        await fs.FlushAsync();
        fs.Close();
        stream.Close();
    }

    public Task Move(string oldPath, string newPath)
    {
        if (Directory.Exists(Path.Combine(BaseDirectory, oldPath)))
        {
            Directory.Move(
                Path.Combine(BaseDirectory, oldPath),
                Path.Combine(BaseDirectory, newPath)
            );
        }
        else
        {
            File.Move(
                Path.Combine(BaseDirectory, oldPath),
                Path.Combine(BaseDirectory, newPath)
            );
        }

        return Task.CompletedTask;
    }

    public Task Delete(string path)
    {
        if (Directory.Exists(Path.Combine(BaseDirectory, path)))
            Directory.Delete(Path.Combine(BaseDirectory, path), true);
        else
            File.Delete(Path.Combine(BaseDirectory, path));

        return Task.CompletedTask;
    }

    public Task CreateDirectory(string path)
    {
        Directory.CreateDirectory(Path.Combine(BaseDirectory, path));
        return Task.CompletedTask;
    }

    public Task<Stream> Read(string path)
    {
        var fs = File.Open(Path.Combine(BaseDirectory, path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return Task.FromResult<Stream>(fs);
    }

    public async Task Download(Func<int, Task> updateProgress, string path, string fileName)
    {
        var stream = await Read(path);
        
        await DownloadService.DownloadStream(
            fileName,
            stream,
            async (bytes, _) =>
            {
                var percent = bytes == 0 ? 0 : (float)bytes / stream.Length * 100;
                await updateProgress.Invoke((int)percent);
            }
        );
        
        stream.Close();
    }

    public async Task Upload(Func<int, Task> updateProgress, string path, Stream stream)
    {
        var progressStream = new ProgressStream(stream, new Progress<long>(async bytes =>
        {
            var percent = bytes == 0 ? 0 : (float)bytes / stream.Length * 100;
            await updateProgress.Invoke((int)percent);
        }));
        
        await Create(path, progressStream);
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
        var destination = Path.Combine(BaseDirectory, path);

        await using var outStream = File.Create(destination);
        await using var gzoStream = new GZipOutputStream(outStream);
        await using var tarStream = new TarOutputStream(gzoStream, Encoding.UTF8);

        foreach (var itemName in itemsToCompress)
        {
            var filePath = Path.Combine(BaseDirectory, itemName);
            var fi = new FileInfo(filePath);

            if (fi.Exists)
                await AddFileToTarGz(tarStream, filePath);
            else
                await AddDirectoryToTarGz(tarStream, Path.Combine(BaseDirectory, itemName));
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
        var destination = Path.Combine(BaseDirectory, path);
        
        await using var outStream = File.Create(destination);
        await using var zipOutputStream = new ZipOutputStream(outStream);
        
        foreach (var itemName in itemsToCompress)
        {
            var filePath = Path.Combine(BaseDirectory, itemName);
            var fi = new FileInfo(filePath);

            if (fi.Exists)
                await AddFileToZip(zipOutputStream, filePath);
            else
                await AddDirectoryToZip(zipOutputStream, Path.Combine(BaseDirectory, itemName));
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
        else if (type.Extension == "zip")
            await DecompressZip(path, destination);
    }

    #region Tar Gz

    private async Task DecompressTarGz(string path, string destination)
    {
        var archivePath = Path.Combine(BaseDirectory, path);

        await using var fs = File.Open(archivePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var gzipInputStream = new GZipInputStream(fs);
        await using var tarInputStream = new TarInputStream(gzipInputStream, Encoding.UTF8);

        while (true)
        {
            var entry = await tarInputStream.GetNextEntryAsync(CancellationToken.None);
            
            if(entry == null)
                break;

            var fileDestination = Path.Combine(BaseDirectory, destination, entry.Name);
            var parentFolder = Path.GetDirectoryName(fileDestination);

            // Ensure parent directory exists, if it's not the base directory
            if (parentFolder != null && parentFolder != BaseDirectory)
                Directory.CreateDirectory(parentFolder);

            await using var fileDestinationFs = File.Open(fileDestination, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            await tarInputStream.CopyToAsync(fileDestinationFs, CancellationToken.None);

            await fileDestinationFs.FlushAsync();
            fileDestinationFs.Close();
        }
        
        tarInputStream.Close();
        gzipInputStream.Close();
        fs.Close();
    }

    #endregion
    
    #region Zip

    private async Task DecompressZip(string path, string destination)
    {
        var archivePath = Path.Combine(BaseDirectory, path);

        await using var fs = File.Open(archivePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var zipInputStream = new ZipInputStream(fs);

        while (true)
        {
            var entry = zipInputStream.GetNextEntry();
            
            if(entry == null)
                break;
            
            if(entry.IsDirectory)
                continue;

            var fileDestination = Path.Combine(BaseDirectory, destination, entry.Name);
            var parentFolder = Path.GetDirectoryName(fileDestination);

            // Ensure parent directory exists, if it's not the base directory
            if (parentFolder != null && parentFolder != BaseDirectory)
                Directory.CreateDirectory(parentFolder);

            await using var fileDestinationFs = File.Open(fileDestination, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            await zipInputStream.CopyToAsync(fileDestinationFs, CancellationToken.None);

            await fileDestinationFs.FlushAsync();
            fileDestinationFs.Close();
        }
        
        zipInputStream.Close();
        fs.Close();
    }

    #endregion
}