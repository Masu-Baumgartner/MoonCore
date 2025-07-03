using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class DeleteOperation : IFileOperation
{
    public string Name => "Delete";
    public string Icon => "icon-trash-2";
    public string ContextCss => "text-error";
    public string ToolbarCss => "btn-error";
    public int Order => 0;
    public bool OnlySingleFile => false;

    public Func<FileEntry, bool>? Filter => _ => true;

    private readonly ToastService ToastService;
    private readonly AlertService AlertService;

    public DeleteOperation(ToastService toastService, AlertService alertService)
    {
        ToastService = toastService;
        AlertService = alertService;
    }

    public async Task Execute(string workingDir, FileEntry[] files, IFileAccess fileAccess, IFileManager fileManager)
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
                                await fileAccess.Delete(
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