using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Exceptions;

namespace MoonCore.Blazor.FlyonUi.Test.Http.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/download")]
public class DownloadController : Controller
{
    [HttpGet]
    public async Task Get([FromQuery] int chunkSize, [FromQuery] int chunkId)
    {
        await using var fs = System.IO.File.Open(
            Path.Combine("testFs", "data.bin"),
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );

        var chunks = fs.Length / chunkSize;
        chunks += fs.Length % chunkSize > 0 ? 1 : 0;

        if (chunkId > chunks)
            throw new HttpApiException("Invalid chunk id: Out of bounds", 400);
        
        var positionToSkipTo = chunkSize * chunkId;
        fs.Position = positionToSkipTo;

        var buffer = new byte[chunkSize];
        var readBytes = await fs.ReadAsync(buffer, 0, chunkSize);
        
        var content = new ByteArrayContent(buffer, 0, readBytes);

        await content.CopyToAsync(Response.Body);
    }
}