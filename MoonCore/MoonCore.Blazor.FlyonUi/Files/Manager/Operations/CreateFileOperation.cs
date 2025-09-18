using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class CreateFileOperation : IToolbarOperation
{
    public string Name => "File";
    public string Icon => "icon-file";
    public string ToolbarCss => "btn-accent";
    public int Order => 1;

    private readonly ModalService ModalService;

    public CreateFileOperation(ModalService modalService)
    {
        ModalService = modalService;
    }
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task ExecuteAsync(string workingDir, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.LaunchAsync<CreateFileModal>(parameters =>
        {
            parameters.Add("OnSubmit", async (string fileName) =>
            {
                await fsAccess.CreateFileAsync(UnixPath.Combine(
                    workingDir,
                    fileName
                ));

                await fileManager.RefreshAsync();
            });
        });
    }
}