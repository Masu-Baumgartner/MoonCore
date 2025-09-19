using Microsoft.Extensions.Logging;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class ArchiveOperation : IMultiFsOperation
{
    public string Name => "Archive";
    public string Icon => "icon-folder-archive";
    public string ContextCss => "text-success";
    public string ToolbarCss => "btn-success";
    public int Order => 0;

    private readonly ToastService ToastService;
    private readonly ModalService ModalService;
    private readonly ILogger<ArchiveOperation> Logger;

    public ArchiveOperation(
        ToastService toastService,
        ModalService modalService,
        ILogger<ArchiveOperation> logger
    )
    {
        ToastService = toastService;
        ModalService = modalService;
        Logger = logger;
    }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => access is IArchiveAccess;

    public async Task ExecuteAsync(string workingDir, FsEntry[] files, IFsAccess fsAccess, IFileManager fileManager)
    {
        var archiveAccess = fsAccess as IArchiveAccess;

        if (archiveAccess == null)
            return;

        await ModalService.LaunchAsync<CreateArchiveModal>(parameters =>
        {
            parameters["Formats"] = archiveAccess.ArchiveFormats;
            parameters["OnSubmit"] = async (string name, ArchiveFormat format) =>
            {
                await ToastService.ProgressAsync(
                    "Creating archive",
                    "Archiving content",
                    async toast =>
                    {
                        try
                        {
                            await archiveAccess.ArchiveAsync(
                                UnixPath.Combine(workingDir, name),
                                format,
                                workingDir,
                                files,
                                async text => await toast.UpdateTextAsync(text)
                            );

                            await ToastService.SuccessAsync("Successfully created archive");
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e, "An error occured while archiving");
                            await ToastService.ErrorAsync("An unhandled error occured while creating archive");
                        }

                        await fileManager.RefreshAsync();
                    }
                );
            };
        });
    }
}