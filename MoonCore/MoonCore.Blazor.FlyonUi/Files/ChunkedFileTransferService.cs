using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Files;

public class ChunkedFileTransferService
{
    private readonly IJSRuntime JsRuntime;

    public delegate Task UploadChunkCallback(int chunkId, ByteArrayContent content);

    public delegate Task<byte[]> DownloadChunkCallback(int chunkId);

    public ChunkedFileTransferService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Upload(
        Stream dataStream,
        long chunkSize,
        UploadChunkCallback callback,
        IProgress<int>? progress = null
    )
    {
        var size = dataStream.Length;

        var chunks = size / chunkSize;
        chunks += size % chunkSize > 0 ? 1 : 0;

        var buffer = new byte[chunkSize];

        for (var chunkId = 0; chunkId < chunks; chunkId++)
        {
            var percent = (int)Math.Round((chunkId + 1f) / chunks * 100);

            progress?.Report(percent);

            var totalRead = 0;

            while (totalRead < chunkSize)
            {
                var bytesToRead = (int)Math.Min(chunkSize - totalRead, buffer.Length - totalRead);
                var bytesRead = await dataStream.ReadAsync(buffer, totalRead, bytesToRead);

                if (bytesRead == 0)
                    break; // EOF

                totalRead += bytesRead;
            }

            var content = new ByteArrayContent(buffer, 0, totalRead);

            await callback.Invoke(
                chunkId,
                content
            );
        }
    }

    public async Task Download(
        string name,
        long chunkSize,
        long size,
        DownloadChunkCallback callback,
        IProgress<int>? progress = null
    )
    {
        var chunks = size / chunkSize;
        chunks += size % chunkSize > 0 ? 1 : 0;

        var id = Random.Shared.Next(0, 1024);

        await JsRuntime.InvokeVoidAsync("moonCore.chunkedDownload.start", id, name);

        for (var chunkId = 0; chunkId < chunks; chunkId++)
        {
            var percent = (int)Math.Round((chunkId + 1f) / chunks * 100);

            progress?.Report(percent);

            var data = await callback.Invoke(chunkId);

            await JsRuntime.InvokeVoidAsync(
                "moonCore.chunkedDownload.writeChunk",
                id,
                data
            );
        }

        await JsRuntime.InvokeVoidAsync("moonCore.chunkedDownload.finish", id);
    }
}