using System.Text;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Helpers;

public class DownloadService
{
    private readonly IJSRuntime JsRuntime;

    public DownloadService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    public async Task Download(string fileName, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await Download(fileName, bytes);
    }

    public async Task Download(string fileName, byte[] data)
    {
        using var stream = new MemoryStream(data);
        await Download(fileName, stream);
    }

    public async Task Download(string fileName, Stream stream)
    {
        using var dotNetStream = new DotNetStreamReference(stream);
        await JsRuntime.InvokeVoidAsync("moonCore.downloadHelper.downloadFileFromStream", fileName, dotNetStream);
    }
    
    public async Task DownloadUrl(string url)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.downloadHelper.downloadFileFromUrl", url);
    }
}