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

    public string ToolbarCss => "";
    public int Order => 0;

    public Func<FsEntry, bool>? Filter => _ => true;
    public FilePermissions RequiredPermissions => FilePermissions.ReadWrite;

    private readonly ToastService ToastService;
    private readonly ModalService ModalService;
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public RenameOperation(ToastService toastService, ModalService modalService)
    {
        ToastService = toastService;
        ModalService = modalService;
    }

    public async Task Execute(string workingDir, FsEntry file, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.Launch<RenameModal>(parameters =>
        {
            parameters["OldName"] = file.Name;
            parameters["OnSubmit"] = async (string newName) =>
            {
                await fsAccess.Move(
                    UnixPath.Combine(workingDir, file.Name),
                    UnixPath.Combine(workingDir, newName)
                );

                await ToastService.Success("Successfully renamed item");
                await fileManager.Refresh();
            };
        });
    }
}