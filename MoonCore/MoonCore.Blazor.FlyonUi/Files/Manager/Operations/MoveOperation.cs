using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class MoveOperation : IFileOperation
{
    public string Name => "Move";
    public string Icon => "icon-folder-input";
    public string ContextCss => "text-primary";
    public string ToolbarCss => "btn-primary";
    public int Order => 0;
    public bool OnlySingleFile => false;
    public Func<FileEntry, bool>? Filter => _ => true;

    private readonly ModalService ModalService;
    private readonly ToastService ToastService;

    public MoveOperation(ModalService modalService, ToastService toastService)
    {
        ModalService = modalService;
        ToastService = toastService;
    }

    public async Task Execute(string workingDir, FileEntry[] files, IFileAccess fileAccess, IFileManager fileManager)
    {
        await ModalService.Launch<MoveModal>(parameters =>
        {
            parameters["Title"] = $"Select a location to move {files.Length} item(s) to";
            parameters["FileAccess"] = fileAccess;
            parameters["InitialPath"] = workingDir;
            parameters["OnSubmit"] = async (string path) =>
            {
                await ToastService.Progress(
                    $"Moving {files.Length} items",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;
                        
                        foreach (var file in files)
                        {
                            await toast.UpdateText(file.Name);

                            try
                            {
                                await fileAccess.Move(
                                    UnixPath.Combine(workingDir, file.Name),
                                    UnixPath.Combine(path, file.Name)
                                );

                                successfully++;
                            }
                            catch (HttpApiException e)
                            {
                                await ToastService.Error(
                                    file.Name,
                                    e.Title
                                );
                            }
                        }
                        
                        await ToastService.Success($"Successfully moved {successfully} item(s)");
                        await fileManager.Refresh();
                    }
                );
            };
        }, "max-w-xl");
    }
}