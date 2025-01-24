using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    private async Task LaunchFileModal()
    {
        await ModalService.Launch<CreateFileModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string fileName) =>
            {
                try
                {
                    await FileSystemProvider.Create(
                        PathBuilder.JoinPaths(CurrentPath, fileName),
                        new MemoryStream([])
                    );

                    await ToastService.Success("Successfully created file");
                    await FileView.Refresh();
                }
                catch (Exception e)
                {
                    Logger.LogError("Unable to create file '{fileName}': {e}", fileName, e);
                    await ToastService.Danger($"Unable to create file: {fileName}");
                }
            });
        });
    }

    private async Task LaunchFolderModal()
    {
        await ModalService.Launch<CreateFolderModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string folderName) =>
            {
                try
                {
                    await FileSystemProvider.CreateDirectory(
                        PathBuilder.JoinPaths(CurrentPath, folderName)
                    );

                    await ToastService.Success("Successfully created folder");
                    await FileView.Refresh();
                }
                catch (Exception e)
                {
                    Logger.LogError("Unable to create folder '{folderName}': {e}", folderName, e);
                    await ToastService.Danger($"Unable to create folder: {folderName}");
                }
            });
        });
    }
}