using System.Text;
using Microsoft.JSInterop;

namespace MoonCoreUI.Services;

public class FileDownloadService
{
    public static string Prefix { get; set; } = "mooncore.utils";
    
    private readonly IJSRuntime JsRuntime;
    
    public FileDownloadService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task DownloadStream(string fileName, Stream stream)
    {
        using var streamRef = new DotNetStreamReference(stream);

        await JsRuntime.InvokeVoidAsync($"{Prefix}.download", fileName, streamRef);
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