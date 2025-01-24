using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    private async Task Download(FileSystemEntry entry)
    {
        if (!entry.IsFile)
        {
            await ToastService.Danger("Folder downloads are not supported at the moment");
            return;
        }

        await ToastService.Info($"Downloading {entry.Name}...");

        var stream = await FileSystemProvider.Read(PathBuilder.JoinPaths(CurrentPath, entry.Name));
        await DownloadService.DownloadStream(entry.Name, stream);

        stream.Close();
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

                await FileView!.Refresh();
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

                    await FileView!.Refresh();
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

                    await FileView!.Refresh();
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
                    await FileView.Refresh();
                });
            }
        );
    }
}