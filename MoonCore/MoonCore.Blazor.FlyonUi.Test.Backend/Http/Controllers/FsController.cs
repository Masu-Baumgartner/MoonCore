using Microsoft.AspNetCore.Mvc;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Request;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;

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
        return PhysicalFile(filePath, contentType, Path.GetFileName(filePath));
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

    [HttpPost("upload")]
    public async Task<IActionResult> UploadChunk(
        [FromQuery] int chunkId,
        [FromQuery] long chunkSize,
        [FromQuery] long fileSize,
        [FromQuery] string fileName,
        IFormFile file)
    {
        var savePath = HandleRawPath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        await using var fs = new FileStream(savePath, chunkId == 0 ? FileMode.Create : FileMode.Append);
        await file.CopyToAsync(fs);

        return Ok();
    }

    [HttpGet("download-chunk")]
    public IActionResult DownloadChunk(string path, int chunkId, long chunkSize)
    {
        var fullPath = HandleRawPath(path);
        using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[chunkSize];
        fs.Seek(chunkId * chunkSize, SeekOrigin.Begin);
        var bytesRead = fs.Read(buffer, 0, buffer.Length);
        return File(buffer.Take(bytesRead).ToArray(), "application/octet-stream");
    }

    [HttpDelete("delete")]
    public IActionResult Delete(string path)
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
}