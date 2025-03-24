using Microsoft.AspNetCore.Components.Web;
using MoonCore.Blazor.Tailwind.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    private ContextMenu ContextMenu;
    private FileSystemEntry ContextMenuEntry;

    private async Task LaunchContextMenu(MouseEventArgs eventArgs, FileSystemEntry entry)
    {
        ContextMenuEntry = entry;
        await ContextMenu.Show(eventArgs.ClientX, eventArgs.ClientY);
    }

    private async Task Download(FileSystemEntry entry)
    {
        if (entry.IsFile)
        {
            var path = PathBuilder.JoinPaths(CurrentPath, entry.Name);
            var name = Path.GetFileName(path);

            await DownloadInternal(path, name);
        }
        else if (CompressProvider != null) // If we have a compress provider, we can help the user out a bit by compressing and then downloading the folder
        {
            var compressType = CompressProvider.CompressTypes.FirstOrDefault();

            if (compressType == null) // Just to make sure
            {
                await ToastService.Danger("Folder downloads are not supported");
                return;
            }

            // We need the temp paths here as the user can navigate away later on, which would change the current path
            var tempArchiveFileName =
                $"folderDownload.{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}." + compressType.Extension;
            var tempArchivePath = PathBuilder.JoinPaths(CurrentPath, tempArchiveFileName);
            var downloadFileName = $"{entry.Name}.{compressType.Extension}";

            await ToastService.Progress(
                $"Downloading {entry.Name}",
                "Compressing folder",
                async toast =>
                {
                    await CompressProvider.Compress(
                        compressType,
                        tempArchivePath,
                        [PathBuilder.JoinPaths(CurrentPath, entry.Name)]
                    );

                    // Reset state
                    //await SetAllSelection(false);
                    //await FileList.Refresh();

                    await toast.Hide();

                    await DownloadInternal(
                        tempArchivePath,
                        downloadFileName,
                        runBlocking: true
                    );

                    await FileSystemProvider.Delete(tempArchivePath);
                }
            );
        }
        else
            await ToastService.Danger("Folder downloads are not supported");
    }

    private async Task DownloadInternal(string path, string name, bool runBlocking = false)
    {
        TaskCompletionSource? tsc = null;

        if (runBlocking)
            tsc = new TaskCompletionSource();

        await ToastService.Launch<DownloadToast>(parameters =>
        {
            parameters.Add("Title", name);
            parameters.Add("Work", async Task (DownloadToast toast) =>
            {
                async Task OnProgressUpdate(int percent)
                {
                    await toast.Update(
                        name,
                        percent
                    );
                }

                await FileSystemProvider.Download(OnProgressUpdate, path, name);

                if (tsc != null)
                    tsc.SetResult();
            });
        });

        if (tsc != null)
            await tsc.Task;
    }

    private async Task Move(FileSystemEntry entry)
    {
        await ModalService.Launch<LocationSelectModal>(size: "max-w-2xl", onConfigure: parameters =>
        {
            parameters.Add("OnSubmit", async (string path) =>
            {
                await FileSystemProvider.Move(
                    PathBuilder.JoinPaths(CurrentPath, entry.Name),
                    PathBuilder.JoinPaths(path, entry.Name)
                );

                await ToastService.Success("Successfully moved item");

                // Reset state
                await FileList.Refresh();
            });

            parameters.Add("FileSystemProvider", FileSystemProvider);
        });
    }

    private async Task Rename(FileSystemEntry entry)
    {
        if (entry.IsFile)
        {
            await ModalService.Launch<RenameFileModal>(size: "max-w-2xl", onConfigure: parameters =>
            {
                parameters.Add("DefaultValue", entry.Name);
                parameters.Add("OnSubmit", async (string val) =>
                {
                    await FileSystemProvider.Move(
                        PathBuilder.JoinPaths(CurrentPath, entry.Name),
                        PathBuilder.JoinPaths(CurrentPath, val)
                    );

                    // Reset state
                    await FileList.Refresh();
                });
            });
        }
        else
        {
            await ModalService.Launch<RenameFolderModal>(size: "max-w-2xl", onConfigure: parameters =>
            {
                parameters.Add("DefaultValue", entry.Name);
                parameters.Add("OnSubmit", async (string val) =>
                {
                    await FileSystemProvider.Move(
                        PathBuilder.JoinPaths(CurrentPath, entry.Name),
                        PathBuilder.JoinPaths(CurrentPath, val)
                    );

                    // Reset state
                    await FileList.Refresh();
                });
            });
        }
    }

    private async Task Delete(FileSystemEntry entry)
    {
        await AlertService.ConfirmDanger(
            "Do you really want to delete this " + (entry.IsFile ? "file" : "folder"),
            "This cannot be undone",
            async () =>
            {
                await ToastService.Progress("Deleting", string.Empty, async _ =>
                {
                    await FileSystemProvider.Delete(PathBuilder.JoinPaths(CurrentPath, entry.Name));

                    // Reset state
                    await FileList.Refresh();
                });
            }
        );
    }
}