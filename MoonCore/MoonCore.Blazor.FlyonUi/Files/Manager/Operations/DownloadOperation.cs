using Microsoft.Extensions.Logging;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Toasts;
using MoonCore.Blazor.FlyonUi.Helpers;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class DownloadOperation : IMultiFsOperation
{
    public string Name => "Download";
    public string Icon => "icon-hard-drive-download";
    public int Order => 0;
    public string ContextCss => "text-primary";
    public string ToolbarCss => "btn-primary";

    private readonly ToastService ToastService;
    private readonly DownloadService DownloadService;
    private readonly ILogger<DownloadOperation> Logger;


    public DownloadOperation(
        ToastService toastService,
        ILogger<DownloadOperation> logger,
        DownloadService downloadService
    )
    {
        ToastService = toastService;
        Logger = logger;
        DownloadService = downloadService;
    }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => access is IDownloadUrlAccess;

    public async Task ExecuteAsync(string workingDir, FsEntry[] entries, IFsAccess access, IFileManager fileManager)
    {
        if (access is not IDownloadUrlAccess downloadUrlAccess)
        {
            await ToastService.ErrorAsync("Unable to download any files/folders. Not supported operation");
            return;
        }

        await ToastService.LaunchAsync<FileDownloadToast>(parameters =>
        {
            parameters["Callback"] = async (FileDownloadToast toast) =>
            {
                var failed = 0;
                var succeeded = 0;

                foreach (var entry in entries)
                {
                    await toast.UpdateStatusAsync($"Starting download for {entry.Name}", 0);

                    var entryPath = UnixPath.Combine(workingDir, entry.Name);

                    try
                    {
                        string url;

                        if (entry.IsFolder)
                            url = await downloadUrlAccess.GetFolderUrlAsync(entryPath);
                        else
                            url = await downloadUrlAccess.GetFileUrlAsync(entryPath);

                        await DownloadService.DownloadUrlAsync(url);

                        succeeded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            e,
                            "An unhandled error occured while downloading item {name}",
                            entry.Name
                        );

                        await ToastService.ErrorAsync(
                            "An error occured while downloading item",
                            entry.Name
                        );

                        failed++;
                    }
                }

                await ToastService.InfoAsync(
                    "File downloads started",
                    $"Successful: {succeeded} - Failed: {failed}"
                );
            };
        });
    }
}