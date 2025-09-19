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

    public async Task ExecuteAsync(string workingDir, FsEntry[] files, IFsAccess fsAccess, IFileManager fileManager)
    {
        var content = "Do you really want to delete: ";
        content += string.Join(", ", files.Take(4).Select(x => x.Name));

        if (files.Length > 4)
            content += $", and {files.Length - 4} more";

        await AlertService.ConfirmDangerAsync(
            $"You you really want to delete {files.Length} item(s)",
            content,
            async () =>
            {
                await ToastService.ProgressAsync(
                    $"Deleing {files.Length} item(s)",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;

                        foreach (var file in files)
                        {
                            await toast.UpdateTextAsync(file.Name);

                            try
                            {
                                await fsAccess.DeleteAsync(
                                    UnixPath.Combine(
                                        workingDir,
                                        file.Name
                                    )
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

                        await ToastService.SuccessAsync($"Successfully deleted {successfully} item(s)");

                        await fileManager.RefreshAsync();
                    }
                );
            }
        );
    }
}