using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class CreateFolderOperation : IToolbarOperation
{
    public string Name => "Folder";
    public string Icon => "icon-folder-plus";
    public string Css => "btn-outline btn-primary";
    public int Order => 0;

    private readonly ModalService ModalService;

    public CreateFolderOperation(ModalService modalService)
    {
        ModalService = modalService;
    }

    public async Task Execute(string workingDir, IFileAccess fileAccess, IFileManager fileManager)
    {
        await ModalService.Launch<CreateDirectoryModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string name) =>
            {
                await fileAccess.CreateDirectory(UnixPath.Combine(
                    workingDir,
                    name
                ));

                await fileManager.Refresh();
            });
        });
    }
}