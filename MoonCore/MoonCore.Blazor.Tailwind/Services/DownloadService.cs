using System.Text;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Services;

public class DownloadService
{
    private readonly IJSRuntime JsRuntime;
    private readonly Dictionary<int, DownloadHandler> Handlers = new();
    private Dictionary<int, Func<long, long, Task>> ProgressHandlers = new();
    private Dictionary<int, Func<bool, Task>> EndHandlers = new();

    public DownloadService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task DownloadUrl(
        string fileName,
        string url,
        Func<long, long, Task>? onProgress = null,
        Func<Task>? onEnd = null,
        Action<Dictionary<string, string>>? onConfigureHeaders = null
    )
    {
        var headers = new Dictionary<string, string>();

        if (onConfigureHeaders != null)
            onConfigureHeaders.Invoke(headers);

        if (onProgress == null &&
            onEnd == null) // Call it without any reporting if no handlers have been defined for it
        {
            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.downloadUrl",
                fileName,
                url,
                null,
                null,
                headers
            );
        }
        else
        {
            var handler = new DownloadHandler()
            {
                OnProgress = onProgress,
                OnComplete = onEnd
            };

            lock (Handlers)
                Handlers[handler.GetHashCode()] = handler;

            await JsRuntime.InvokeVoidAsync(
                "moonCoreDownloadService.downloadUrl",
                fileName,
                url,
                DotNetObjectReference.Create(this),
                handler.GetHashCode(),
                headers
            );
        }
    }

    public async Task DownloadStream(
        string fileName,
        Stream stream,
        Func<long, long, Task>? onProgress = null,
        Func<Task>? onEnd = null
    )
    {
        using var streamRef = new DotNetStreamReference(stream, true);

        if (onProgress == null && onEnd == null)
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
            var handler = new DownloadHandler()
            {
                OnProgress = onProgress,
                OnComplete = onEnd
            };

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
    public async Task ReceiveReport(int id, long loaded, long total, bool end)
    {
        DownloadHandler? handler;

        lock (Handlers)
            handler = Handlers.GetValueOrDefault(id);

        if (handler == null)
            return;

        if (handler.OnProgress != null)
            await handler.OnProgress.Invoke(loaded, total);

        if (end && handler.OnComplete != null)
            await handler.OnComplete.Invoke();

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

    private class DownloadHandler
    {
        public Func<long, long, Task>? OnProgress { get; set; }
        public Func<Task>? OnComplete { get; set; }
    }
}