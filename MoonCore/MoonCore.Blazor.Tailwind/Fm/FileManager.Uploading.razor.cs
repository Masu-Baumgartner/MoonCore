using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MoonCore.Blazor.Tailwind.Fm.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    [Parameter] public int MaxUploadSize { get; set; } = 100;

    private DateTime LastDragEventHandledAt = DateTime.MinValue;

    private bool IsUploading = false;
    private int UploadedCount = 0;
    private int LeftCount = 0;

    private async Task TriggerUpload()
    {
        if (IsUploading)
        {
            await ToastService.Success("Added files to upload queue");
            return;
        }

        IsUploading = true;
        UploadedCount = 0;
        LeftCount = 0;

        var uploadSizeInBytes = ByteConverter.FromMegaBytes(MaxUploadSize).Bytes;

        async Task Work(UploadToast toast)
        {
            while (true)
            {
                var nextItem = await JsRuntime.InvokeAsync<TransferData2?>(
                    "moonCoreFileManager.getNextFromCache",
                    []
                );

                if (nextItem == null)
                {
                    IsUploading = false;
                    
                    await toast.Update(
                        $"Uploading",
                        "0 MB/s",
                        UploadedCount,
                        LeftCount
                    );
                    
                    break;
                }

                if (nextItem.Stream != null)
                {
                    await HandleUpload(
                        nextItem.Path,
                        async () => await nextItem.Stream.OpenReadStreamAsync(uploadSizeInBytes)
                    );
                }

                LeftCount = nextItem.Left;
                UploadedCount++;

                var name = Path.GetFileName(nextItem.Path);
                
                await toast.Update(
                    $"Uploading: {name}",
                    "0 MB/s",
                    UploadedCount,
                    LeftCount
                );
            }
        }

        await ToastService.Launch<UploadToast>(parameters =>
        {
            parameters.Add("Title", "Uploading");
            parameters.Add("Work", Work);
        });

        Console.WriteLine("AAAAAAAA END");
    }

    private async Task HandleUpload(string path, Func<Task<Stream>> streamCallback)
    {
        Stream? stream = null;

        try
        {
            stream = await streamCallback.Invoke();

            var pathToUpload = PathBuilder.JoinPaths(CurrentPath, path);
            await FileSystemProvider.Create(pathToUpload, stream);
        }
        catch (ArgumentOutOfRangeException)
        {
            await ToastService.Danger($"Unable to upload file {path}: The provided file is too big");
        }
        catch (Exception e)
        {
            await ToastService.Danger($"Unable to upload file {path}: An unknown error occured");
            Logger.LogError("Unable to upload receive file '{path}': {e}", path, e);
        }
        finally
        {
            stream?.Close();
        }
    }

    private async Task LaunchUploadModal()
    {
        await ModalService.Launch<UploadModal>(size: "max-w-xl", allowUnfocusHide: true, onConfigure: parameters =>
        {
            parameters.Add("OnUpload", async Task (string path, Stream dataStream) =>
            {
                var pathToUpload = PathBuilder.JoinPaths(CurrentPath, path);
                await FileSystemProvider.Create(pathToUpload, dataStream);
            });

            parameters.Add("OnCache", TriggerUpload);

            parameters.Add("OnUploadCompleted", async () =>
            {
                // Reset state
                await SetAllSelection(false);
                await FileList.Refresh();
            });
        });
    }

    private async Task HandleDrag()
    {
        // Debounce time of one second
        if ((DateTime.UtcNow - LastDragEventHandledAt).TotalSeconds < 1)
            return;

        LastDragEventHandledAt = DateTime.UtcNow;

        await LaunchUploadModal();
    }
}