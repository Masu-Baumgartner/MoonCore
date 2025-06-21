using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Files;

public class ChunkedFileTransferService
{
    private readonly IJSRuntime JsRuntime;

    public ChunkedFileTransferService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Upload(
        Func<int, Task> updateProgress,
        Stream dataStream,
        long chunkSize,
        Func<int, ByteArrayContent, Task> onHandle
    )
    {
        var size = dataStream.Length;

        var chunks = size / chunkSize;
        chunks += size % chunkSize > 0 ? 1 : 0;

        for (var chunkId = 0; chunkId < chunks; chunkId++)
        {
            var percent = (int)Math.Round((chunkId + 1f) / chunks * 100);
            await updateProgress.Invoke(percent);

            var buffer = new byte[chunkSize];
            var bytesRead = await dataStream.ReadAsync(buffer);

            var content = new ByteArrayContent(buffer, 0, bytesRead);

            await onHandle.Invoke(
                chunkId,
                content
            );
        }
    }

    public async Task Download(
        Func<int, Task> updateProgress,
        string name,
        long chunkSize,
        long size,
        Func<int, Task<byte[]>> onHandle
    )
    {
        var chunks = size / chunkSize;
        chunks += size % chunkSize > 0 ? 1 : 0;

        var id = Random.Shared.Next(0, 1024);

        await JsRuntime.InvokeVoidAsync("moonCore.chunkedDownload.start", id, name);

        for (var chunkId = 0; chunkId < chunks; chunkId++)
        {
            var percent = (int)Math.Round((chunkId + 1f) / chunks * 100);
            await updateProgress.Invoke(percent);

            var data = await onHandle.Invoke(chunkId);

            await JsRuntime.InvokeVoidAsync(
                "moonCore.chunkedDownload.writeChunk",
                id,
                data
            );
        }
        
        await JsRuntime.InvokeVoidAsync("moonCore.chunkedDownload.finish", id);
    }
}