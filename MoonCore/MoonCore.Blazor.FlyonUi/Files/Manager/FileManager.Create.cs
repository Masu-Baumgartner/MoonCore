using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private async Task CreateNewFile()
    {
        await ModalService.Launch<CreateFileModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string fileName) =>
            {
                await FileAccess.CreateFile(UnixPath.Combine(
                    CurrentPath,
                    fileName
                ));

                await FileView.Refresh();
            });
        });
    }

    private async Task CreateNewDirectory()
    {
        await ModalService.Launch<CreateDirectoryModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string name) =>
            {
                await FileAccess.CreateDirectory(UnixPath.Combine(
                    CurrentPath,
                    name
                ));

                await FileView.Refresh();
            });
        });
    }
}