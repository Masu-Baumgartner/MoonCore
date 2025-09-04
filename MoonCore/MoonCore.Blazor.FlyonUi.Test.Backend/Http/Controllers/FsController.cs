using System.IO.Enumeration;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Request;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[ApiController]
[Route("api/fs")]
public class FsController : Controller
{
    private readonly string RootDirectory;

    public FsController()
    {
        RootDirectory = Path.Combine("testFs");
        Directory.CreateDirectory(RootDirectory);
    }

    private string HandleRawPath(string path)
        => Path.Combine(RootDirectory, path.TrimStart('/'));

    [HttpPost("create-file")]
    public IActionResult CreateFile(string path)
    {
        var fs = System.IO.File.Create(HandleRawPath(path));
        fs.Close();
        return Ok();
    }

    [HttpPost("create-directory")]
    public IActionResult CreateDirectory(string path)
    {
        Directory.CreateDirectory(HandleRawPath(path));
        return Ok();
    }

    [HttpGet("list")]
    public ActionResult<FsEntryResponse[]> List(string path)
    {
        var entries = Directory.GetFileSystemEntries(HandleRawPath(path));

        var result = entries.Select(entry =>
        {
            var fi = new FileInfo(entry);
            if (fi.Exists)
            {
                return new FsEntryResponse
                {
                    Name = fi.Name,
                    IsFolder = false,
                    Size = fi.Length,
                    CreatedAt = fi.CreationTimeUtc,
                    UpdatedAt = fi.LastWriteTimeUtc
                };
            }

            var di = new DirectoryInfo(entry);

            return new FsEntryResponse
            {
                Name = di.Name,
                IsFolder = true,
                Size = 0,
                CreatedAt = di.CreationTimeUtc,
                UpdatedAt = di.LastWriteTimeUtc
            };
        }).ToArray();

        return Ok(result);
    }

    [HttpPost("move")]
    public IActionResult Move(string oldPath, string newPath)
    {
        var oldPathSafe = HandleRawPath(oldPath);
        var newPathSafe = HandleRawPath(newPath);

        if (System.IO.File.Exists(oldPathSafe))
            System.IO.File.Move(oldPathSafe, newPathSafe);
        else
            Directory.Move(oldPathSafe, newPathSafe);

        return Ok();
    }

    [HttpGet("download")]
    public IActionResult Download(string path)
    {
        var filePath = HandleRawPath(path);
        var contentType = "application/octet-stream";
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

        var fs = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        return File(fs, contentType, Path.GetFileName(filePath), true);
    }

    [HttpPost("upload/single")]
    public async Task<IActionResult> UploadSingle([FromQuery] string filePath, IFormFile file)
    {
        var savePath = HandleRawPath(filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        await using var fs = new FileStream(savePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return Ok();
    }

    [HttpDelete("delete")]
    public IActionResult Delete([FromQuery] string path)
    {
        var pathSafe = HandleRawPath(path);
        if (System.IO.File.Exists(pathSafe))
            System.IO.File.Delete(pathSafe);
        else
            Directory.Delete(pathSafe, true);
        return Ok();
    }

    [HttpPost("combine")]
    public async Task<IActionResult> Combine([FromBody] FsCombineRequest request)
    {
        var dest = HandleRawPath(request.Destination);
        await using var fs = System.IO.File.Open(dest, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

        foreach (var file in request.Files)
        {
            var filePath = HandleRawPath(file);

            await using var readFs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await readFs.CopyToAsync(fs);
            await fs.FlushAsync();
            readFs.Close();
        }

        await fs.FlushAsync();
        fs.Close();

        return Ok();
    }
    
    [HttpPost("compress")]
    public async Task<IResult> Compress([FromBody] FsCompressRequest request)
    {
        var destination = HandleRawPath(request.Destination);
        var rootPath = HandleRawPath(request.Root);
        var files = request.Files.Select(x => Path.Combine(rootPath, x));

        switch (request.Identifier)
        {
            case "zip":
                await CompressZip(destination, files, rootPath);
                break;
            
            default:
                return Results.Problem("Invalid compress identifier provided", statusCode: 400);
        }

        return Results.Ok();
    }

    private async Task CompressZip(string destination, IEnumerable<string> files, string root)
    {
        await using var fs = System.IO.File.Open(
            destination,
            FileMode.Create,
            FileAccess.ReadWrite,
            FileShare.Read
        );

        await using var zipStream = new ZipOutputStream(fs)
        {
            IsStreamOwner = false
        };

        foreach (var file in files)
        {
            await CompressZipInternal(zipStream, file, root);
        }

        await zipStream.FlushAsync();
        await fs.FlushAsync();
        
        zipStream.Close();
        fs.Close();
    }

    private async Task CompressZipInternal(ZipOutputStream stream, string file, string root)
    {
        if (Directory.Exists(file)) // Check if it's a directory
        {
            foreach (var directory in Directory.EnumerateFileSystemEntries(file))
                await CompressZipInternal(stream, directory, root);
        }
        else
        {
            var fi = new FileInfo(file);
            
            if(!fi.Exists)
                return;

            var relativePath = Formatter.ReplaceStart(file, root, "");

            await stream.PutNextEntryAsync(new ZipEntry(relativePath)
            {
                Size = fi.Length,
                DateTime = fi.LastWriteTime
            });

            await using var fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await fs.CopyToAsync(stream);
            fs.Close();

            await stream.CloseEntryAsync(CancellationToken.None);
        }
    }
}