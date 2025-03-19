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

        await ToastService.Launch<UploadToast>(parameters =>
        {
            parameters.Add("Title", "Uploading");
            parameters.Add("Work", UploadFiles);
        });
    }

    private async Task UploadFiles(UploadToast toast)
    {
        var uploadSizeInBytes = ByteConverter.FromMegaBytes(MaxUploadSize).Bytes;
        var path = CurrentPath + ""; // Clone path so the upload target so the user cannot change it anymore

        while (true)
        {
            var nextItem = await JsRuntime.InvokeAsync<TransferData?>(
                "moonCoreFileManager.getNextFromCache",
                []
            );

            if (nextItem == null)
            {
                IsUploading = false;

                await toast.Update(
                    "Uploading",
                    -1,
                    UploadedCount,
                    LeftCount
                );

                break;
            }

            var name = Path.GetFileName(nextItem.Path);

            if (nextItem.Stream != null)
            {
                LeftCount = nextItem.Left;
                Stream? stream = null;

                try
                {
                    var pathToUpload = PathBuilder.JoinPaths(path, nextItem.Path);
                    stream = await nextItem.Stream.OpenReadStreamAsync(uploadSizeInBytes);

                    async Task OnProgressUpdate(int percent)
                    {
                        await toast.Update(
                            $"Uploading {name}",
                            percent,
                            UploadedCount,
                            LeftCount
                        );
                    }

                    await FileSystemProvider.Upload(OnProgressUpdate, pathToUpload, stream);
                }
                catch (ArgumentOutOfRangeException)
                {
                    await ToastService.Danger($"Unable to upload file {name}: The provided file is too big");
                }
                catch (Exception e)
                {
                    await ToastService.Danger($"Unable to upload file {name}: An unknown error occured");
                    Logger.LogError("Unable to upload receive file '{path}': {e}", name, e);
                }
                finally
                {
                    stream?.Close();
                }
            }
            else
            {
                LeftCount = nextItem.Left;
                UploadedCount++;
                
                await toast.Update(
                    $"Uploading: {name}",
                    -1,
                    UploadedCount,
                    LeftCount
                );
            }
        }

        // Reset state
        await SetAllSelection(false);
        await FileList.Refresh();
    }

    private async Task LaunchUploadModal()
    {
        await ModalService.Launch<UploadModal>(size: "max-w-xl", allowUnfocusHide: true,
            onConfigure: parameters => { parameters.Add("OnTriggerUpload", TriggerUpload); });
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