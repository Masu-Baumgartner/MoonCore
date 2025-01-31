using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    private DateTime LastDragEventHandledAt = DateTime.MinValue;

    private async Task LaunchUploadModal()
    {
        await ModalService.Launch<UploadModal>(size: "max-w-xl", allowUnfocusHide: true, onConfigure: parameters =>
        {
            parameters.Add("OnUpload", async Task (string path, Stream dataStream) =>
            {
                var pathToUpload = PathBuilder.JoinPaths(CurrentPath, path);
                await FileSystemProvider.Create(pathToUpload, dataStream);
            });

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