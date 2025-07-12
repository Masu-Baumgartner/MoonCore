using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class CreateFileOperation : IToolbarOperation
{
    public string Name => "File";
    public string Icon => "icon-file-plus-2";
    public string ToolbarCss => "btn-outline btn-primary";
    public int Order => 1;

    private readonly ModalService ModalService;

    public CreateFileOperation(ModalService modalService)
    {
        ModalService = modalService;
    }
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task Execute(string workingDir, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.Launch<CreateFileModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string fileName) =>
            {
                await fsAccess.CreateFile(UnixPath.Combine(
                    workingDir,
                    fileName
                ));

                await fileManager.Refresh();
            });
        });
    }
}