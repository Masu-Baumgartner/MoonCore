using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class DeleteOperation : IMultiFsOperation
{
    public string Name => "Delete";
    public string Icon => "icon-trash-2";
    public string ContextCss => "text-error";
    public string ToolbarCss => "btn-error";
    public FilePermissions RequiredPermissions => FilePermissions.ReadWrite;
    public int Order => -10;

    private readonly ToastService ToastService;
    private readonly AlertService AlertService;

    public DeleteOperation(ToastService toastService, AlertService alertService)
    {
        ToastService = toastService;
        AlertService = alertService;
    }
    
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task Execute(string workingDir, FsEntry[] files, IFsAccess fsAccess, IFileManager fileManager)
    {
        var content = "Do you really want to delete: ";
        content += string.Join(", ", files.Take(4).Select(x => x.Name));

        if (files.Length > 4)
            content += $", and {files.Length - 4} more";

        await AlertService.ConfirmDanger(
            $"You you really want to delete {files.Length} item(s)",
            content,
            async () =>
            {
                await ToastService.Progress(
                    $"Deleing {files.Length} item(s)",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;

                        foreach (var file in files)
                        {
                            await toast.UpdateText(file.Name);

                            try
                            {
                                await fsAccess.Delete(
                                    UnixPath.Combine(
                                        workingDir,
                                        file.Name
                                    )
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

                        await ToastService.Success($"Successfully deleted {successfully} item(s)");

                        await fileManager.Refresh();
                    }
                );
            }
        );
    }
}