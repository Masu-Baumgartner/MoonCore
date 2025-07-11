using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class CreateFolderOperation : IToolbarOperation
{
    public string Name => "Folder";
    public string Icon => "icon-folder-plus";
    public string ToolbarCss => "btn-outline btn-primary";
    public int Order => 0;

    private readonly ModalService ModalService;

    public CreateFolderOperation(ModalService modalService)
    {
        ModalService = modalService;
    }
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task Execute(string workingDir, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.Launch<CreateDirectoryModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string name) =>
            {
                await fsAccess.CreateDirectory(UnixPath.Combine(
                    workingDir,
                    name
                ));

                await fileManager.Refresh();
            });
        });
    }
}