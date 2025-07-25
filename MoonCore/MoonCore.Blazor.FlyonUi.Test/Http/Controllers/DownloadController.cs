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
    public async Task Get([FromQuery] int chunkSize, [FromQuery] int chunkId, [FromQuery] string path)
    {
        try
        {
            await using var fs = System.IO.File.Open(
                Path.Combine("testFs", path.Replace('/', Path.DirectorySeparatorChar)),
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

            var remainingBytes = fs.Length - positionToSkipTo;
            var bytesToRead = (int)Math.Min(chunkSize, remainingBytes);

            var buffer = new byte[bytesToRead];
            var readBytes = await fs.ReadAsync(buffer.AsMemory(0, bytesToRead));

            Response.ContentLength = readBytes;
            await Response.Body.WriteAsync(buffer.AsMemory(0, bytesToRead));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}