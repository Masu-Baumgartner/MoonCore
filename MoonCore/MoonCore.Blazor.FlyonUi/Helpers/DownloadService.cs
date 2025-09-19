using System.Text;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Helpers;

/// <summary>
/// Provides functionality to download content on the browser.
/// Requires the interop javascript to work
/// </summary>
public class DownloadService
{
    private readonly IJSRuntime JsRuntime;

    public DownloadService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Downloads a string as a text file with the provided filename
    /// </summary>
    /// <param name="fileName">Filename of the download</param>
    /// <param name="text">Content to download</param>
    public async Task DownloadAsync(string fileName, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await DownloadAsync(fileName, bytes);
    }

    /// <summary>
    /// Downloads a byte array as a file
    /// </summary>
    /// <param name="fileName">Filename of the download</param>
    /// <param name="data">Content to download</param>
    public async Task DownloadAsync(string fileName, byte[] data)
    {
        using var stream = new MemoryStream(data);
        await DownloadAsync(fileName, stream);
    }

    /// <summary>
    /// Downloads the provided stream as a file
    /// </summary>
    /// <param name="fileName">Filename of the download</param>
    /// <param name="stream">Stream to retrieve the content from</param>
    public async Task DownloadAsync(string fileName, Stream stream)
    {
        using var dotNetStream = new DotNetStreamReference(stream);
        await JsRuntime.InvokeVoidAsync("moonCore.downloadHelper.downloadFileFromStream", fileName, dotNetStream);
    }
    
    /// <summary>
    /// Downloads a file from the provided url
    /// </summary>
    /// <param name="url">URL to download from</param>
    public async Task DownloadUrlAsync(string url)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.downloadHelper.downloadFileFromUrl", url);
    }
}