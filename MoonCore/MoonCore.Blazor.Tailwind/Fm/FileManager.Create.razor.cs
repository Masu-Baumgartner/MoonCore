using Microsoft.Extensions.Logging;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

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
                        UnixPath.Combine(CurrentPath, fileName),
                        new MemoryStream([])
                    );

                    await ToastService.Success("Successfully created file");
                    
                    // Reset state
                    await FileList.Refresh();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Unable to create file '{fileName}'", fileName);
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
                        UnixPath.Combine(CurrentPath, folderName)
                    );

                    await ToastService.Success("Successfully created folder");
                    
                    // Reset state
                    await FileList.Refresh();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Unable to create folder '{folderName}'", folderName);
                    await ToastService.Danger($"Unable to create folder: {folderName}");
                }
            });
        });
    }
}