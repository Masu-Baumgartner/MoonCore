using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class CreateFileOperation : IToolbarOperation
{
    public string Name => "File";
    public string Icon => "icon-folder-plus";
    public string Css => "btn-outline btn-primary";
    public int Order => 1;

    private readonly ModalService ModalService;

    public CreateFileOperation(ModalService modalService)
    {
        ModalService = modalService;
    }

    public async Task Execute(string workingDir, IFileAccess fileAccess, IFileManager fileManager)
    {
        await ModalService.Launch<CreateFileModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string fileName) =>
            {
                await fileAccess.CreateFile(UnixPath.Combine(
                    workingDir,
                    fileName
                ));

                await fileManager.Refresh();
            });
        });
    }
}