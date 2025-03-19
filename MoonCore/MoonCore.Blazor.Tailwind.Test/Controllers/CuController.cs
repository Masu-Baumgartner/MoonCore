using Microsoft.AspNetCore.Mvc;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.Controllers;

[ApiController]
[Route("api/cu")]
public class CuController : Controller
{
    private readonly long ChunkSize = ByteConverter.FromMegaBytes(50).Bytes;

    [HttpPost]
    public async Task UploadChunk([FromQuery] string path, [FromQuery] long totalSize, [FromQuery] int chunkId)
    {
        if (Request.Form.Files.Count != 1)
            throw new HttpApiException("Invalid amount of files specified", 400);

        var file = Request.Form.Files.First();

        if (file.Length > ChunkSize)
            throw new HttpApiException("The provided data exceeds the chunk size limit", 400);

        var lastChunkId = totalSize == 0 ? 0 : (totalSize - 1) / ChunkSize;

        if (chunkId > lastChunkId)
            throw new HttpApiException("Invalid chunk id: Out of bounds", 400);

        var positionToSkipTo = ChunkSize * chunkId;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "testFs", path);
        var dir = Path.GetDirectoryName(filePath)!;

        Directory.CreateDirectory(dir);

        var didExistBefore = System.IO.File.Exists(filePath);

        await using var fs = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

        if (!didExistBefore)
            fs.SetLength(totalSize);

        fs.Position = positionToSkipTo;

        var dataStream = file.OpenReadStream();

        await dataStream.CopyToAsync(fs);
        await fs.FlushAsync();

        fs.Close();
    }
}