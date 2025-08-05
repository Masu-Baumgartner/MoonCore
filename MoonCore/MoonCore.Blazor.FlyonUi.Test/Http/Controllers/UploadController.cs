using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Blazor.FlyonUi.Test.Http.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/upload")]
public class UploadController : Controller
{
    [HttpPost]
    public async Task Post([FromQuery] string fileName, [FromQuery] long fileSize, [FromQuery] int chunkSize, [FromQuery] int chunkId)
    {
        try
        {
            var path = Path.Combine(
                "testFs", fileName.TrimStart('/')
            );

            var parentDir = Path.GetDirectoryName(path);
            
            if(parentDir != null)
                Directory.CreateDirectory(parentDir);

            var fileStream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

            if (fileStream.Length != fileSize)
                fileStream.SetLength(fileSize);

            var positionToSkipTo = chunkSize * chunkId;
            fileStream.Position = positionToSkipTo;

            Console.WriteLine($"Position 1: {fileStream.Position}");

            var file = Request.Form.Files[0];

            Console.WriteLine($"Received: {chunkId} with size {file.Length}");

            await using var dataStream = file.OpenReadStream();

            await dataStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            Console.WriteLine($"Position 2: {fileStream.Position}");

            fileStream.Close();
            dataStream.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("single")]
    public async Task PostSingle([FromQuery] string filePath)
    {
        try
        {
            var path = Path.Combine(
                "testFs", filePath.TrimStart('/')
            );

            var parentDir = Path.GetDirectoryName(path);
            
            if(parentDir != null)
                Directory.CreateDirectory(parentDir);

            var fileStream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            
            var file = Request.Form.Files[0];
            await using var dataStream = file.OpenReadStream();

            await dataStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            Console.WriteLine($"Position 2: {fileStream.Position}");

            fileStream.Close();
            dataStream.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}