using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class RenameOperation : ISingleFsOperation
{
    public string Name => "Rename";
    public string Icon => "icon-folder-pen";
    public string ContextCss => "text-primary";
    
    public int Order => 0;

    public Func<FsEntry, bool>? Filter => _ => true;

    private readonly ToastService ToastService;
    private readonly ModalService ModalService;
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public RenameOperation(ToastService toastService, ModalService modalService)
    {
        ToastService = toastService;
        ModalService = modalService;
    }

    public async Task ExecuteAsync(string workingDir, FsEntry file, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.LaunchAsync<RenameModal>(parameters =>
        {
            parameters["OldName"] = file.Name;
            parameters["OnSubmit"] = async (string newName) =>
            {
                await fsAccess.MoveAsync(
                    UnixPath.Combine(workingDir, file.Name),
                    UnixPath.Combine(workingDir, newName)
                );

                await ToastService.SuccessAsync("Successfully renamed item");
                await fileManager.RefreshAsync();
            };
        });
    }
}