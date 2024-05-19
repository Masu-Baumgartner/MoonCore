using System.Text;
using Microsoft.JSInterop;
using MoonCoreUI.Extensions;
using MoonCoreUI.Models;

namespace MoonCoreUI.Services;

public class FileDownloadService
{
    private readonly MoonCoreUiConfiguration Configuration;
    private readonly IJSRuntime JsRuntime;
    
    public FileDownloadService(IJSRuntime jsRuntime, MoonCoreUiConfiguration configuration)
    {
        JsRuntime = jsRuntime;
        Configuration = configuration;
    }

    public async Task DownloadStream(string fileName, Stream stream)
    {
        using var streamRef = new DotNetStreamReference(stream);

        await JsRuntime.InvokeVoidAsyncHandled($"{Configuration.FileDownloadJavascriptPrefix}.download", fileName, streamRef);
    }

    public async Task DownloadBytes(string fileName, byte[] bytes)
    {
        var ms = new MemoryStream(bytes);
        
        await DownloadStream(fileName, ms);
        
        ms.Close();
        await ms.DisposeAsync();
    }

    public async Task DownloadString(string fileName, string content) =>
        await DownloadBytes(fileName, Encoding.UTF8.GetBytes(content));
}