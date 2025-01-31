using Microsoft.AspNetCore.Components.Web;
using MoonCore.Blazor.Tailwind.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    private ContextMenu ContextMenu;
    private FileSystemEntry ContextMenuEntry;

    private async Task LaunchContextMenu(MouseEventArgs eventArgs, FileSystemEntry entry, bool useOffset = false)
    {
        ContextMenuEntry = entry;

        if (useOffset)
            await ContextMenu.Show(eventArgs.ClientX - 150, eventArgs.ClientY - 10);
        else
            await ContextMenu.Show(eventArgs.ClientX, eventArgs.ClientY);
    }

    private async Task Download(FileSystemEntry entry)
    {
        if (entry.IsFile)
        {
            await ToastService.Info($"Downloading {entry.Name}...");

            var stream = await FileSystemProvider.Read(PathBuilder.JoinPaths(CurrentPath, entry.Name));
            await DownloadService.DownloadStream(entry.Name, stream);

            stream.Close();
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
                    await SetAllSelection(false);
                    await FileList.Refresh();

                    await toast.UpdateText("Downloading folder");

                    var stream = await FileSystemProvider.Read(tempArchivePath);
                    await DownloadService.DownloadStream($"{entry.Name}.{compressType.Extension}", stream);

                    await ToastService.Success("Downloading folder",
                        $"You can delete the '{tempArchiveFileName}' after the download has finished");
                }
            );
        }
        else
            await ToastService.Danger("Folder downloads are not supported");
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
                await SetAllSelection(false);
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
                    await SetAllSelection(false);
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
                    await SetAllSelection(false);
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
                    await SetAllSelection(false);
                    await FileList.Refresh();
                });
            }
        );
    }
}