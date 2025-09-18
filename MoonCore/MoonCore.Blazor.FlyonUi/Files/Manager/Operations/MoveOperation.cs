using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class MoveOperation : IMultiFsOperation
{
    public string Name => "Move";
    public string Icon => "icon-folder-input";
    public string ContextCss => "text-primary";
    public string ToolbarCss => "btn-primary";
    public int Order => 0;

    private readonly ModalService ModalService;
    private readonly ToastService ToastService;

    public MoveOperation(ModalService modalService, ToastService toastService)
    {
        ModalService = modalService;
        ToastService = toastService;
    }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task ExecuteAsync(string workingDir, FsEntry[] files, IFsAccess fsAccess, IFileManager fileManager)
    {
        await ModalService.LaunchAsync<MoveModal>(parameters =>
        {
            parameters["FsAccess"] = fsAccess;
            parameters["InitialPath"] = workingDir;
            parameters["OnSubmit"] = async (string path) =>
            {
                await ToastService.ProgressAsync(
                    $"Moving {files.Length} items",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;
                        
                        foreach (var file in files)
                        {
                            await toast.UpdateTextAsync(file.Name);

                            try
                            {
                                await fsAccess.MoveAsync(
                                    UnixPath.Combine(workingDir, file.Name),
                                    UnixPath.Combine(path, file.Name)
                                );

                                successfully++;
                            }
                            catch (HttpApiException e)
                            {
                                await ToastService.ErrorAsync(
                                    file.Name,
                                    e.Title
                                );
                            }
                        }
                        
                        await ToastService.SuccessAsync($"Successfully moved {successfully} item(s)");
                        await fileManager.RefreshAsync();
                    }
                );
            };
        }, "max-w-xl");
    }
}