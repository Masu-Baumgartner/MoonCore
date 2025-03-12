using System.Text;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Services;

public class DownloadService
{
    private readonly IJSRuntime JsRuntime;
    private Dictionary<int, Func<long, bool, Task>> Handlers = new();

    public DownloadService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task DownloadUrl(string fileName, string url, Func<long, bool, Task>? handler = null)
    {
        if (handler == null)
        {
            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.downloadUrl",
                fileName,
                url,
                null,
                null
            );
        }
        else
        {
            lock (Handlers)
                Handlers[handler.GetHashCode()] = handler;
            
            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.downloadUrl",
                fileName,
                url,
                DotNetObjectReference.Create(this),
                handler.GetHashCode()
            );
        }
    }

    public async Task DownloadStream(string fileName, Stream stream, Func<long, bool, Task>? handler = null)
    {
        using var streamRef = new DotNetStreamReference(stream, true);

        if (handler == null)
        {
            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.download",
                fileName,
                streamRef,
                null,
                null
            );
        }
        else
        {
            lock (Handlers)
                Handlers[handler.GetHashCode()] = handler;

            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.download",
                fileName,
                streamRef,
                handler.GetHashCode(),
                DotNetObjectReference.Create(this)
            );
        }
    }

    [JSInvokable]
    public async Task ReceiveReport(int id, long downloadedBytes, bool end)
    {
        Func<long, bool, Task>? handler = null;

        lock (Handlers)
            handler = Handlers.GetValueOrDefault(id);

        if (handler == null)
            return;

        await handler.Invoke(downloadedBytes, end);

        if (end)
        {
            lock (Handlers)
                Handlers.Remove(id);
        }
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