using Microsoft.Extensions.Logging;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Toasts;
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
    private readonly ILogger<DownloadOperation> Logger;
    private readonly ChunkedFileTransferService FileTransferService;

    public DownloadOperation(
        ToastService toastService,
        ILogger<DownloadOperation> logger,
        ChunkedFileTransferService fileTransferService
    )
    {
        ToastService = toastService;
        Logger = logger;
        FileTransferService = fileTransferService;
    }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => true;

    public async Task Execute(string workingDir, FsEntry[] entries, IFsAccess access, IFileManager fileManager)
    {
        await ToastService.Launch<FileDownloadToast>(parameters =>
        {
            parameters["Callback"] = async (FileDownloadToast toast) =>
            {
                var failed = 0;
                var succeeded = 0;

                foreach (var entry in entries)
                {
                    try
                    {
                        if (entry.IsFolder)
                        {
                            if (access is not IArchiveAccess archiveAccess || archiveAccess.ArchiveFormats.Length  == 0)
                            {
                                await ToastService.Error("The download of folders is not supported");
                                continue;
                            }
                            
                            await toast.UpdateStatus("Archiving", 0);

                            var format = archiveAccess.ArchiveFormats.First();
                            var tmpName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.tmp";
                            var tmpPath = UnixPath.Combine(workingDir, tmpName);

                            await archiveAccess.Archive(
                                tmpPath,
                                format,
                                workingDir,
                                [entry],
                                async percent =>
                                {
                                    await toast.UpdateStatus("Archiving", percent);
                                }
                            );

                            var files = await access.List(workingDir);
                            var archiveFsEntry = files.FirstOrDefault(x => x.Name == tmpName);

                            if (archiveFsEntry == null)
                            {
                                await ToastService.Error($"Unable to create download archive for: {entry.Name}");
                                failed++;
                                continue;
                            }

                            var downloadName = entry.Name + format.Extensions[0];

                            await DownloadFile(
                                tmpPath,
                                downloadName,
                                archiveFsEntry.Size,
                                toast, access, fileManager
                            );

                            await toast.UpdateStatus("Removing download archive", 0);

                            await access.Delete(tmpPath);
                        }
                        else
                        {
                            await DownloadFile(
                                UnixPath.Combine(workingDir, entry.Name),
                                entry.Name,
                                entry.Size,
                                toast, access, fileManager
                            );
                        }

                        succeeded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            "An unhandled error occured while downloading item {name}: {e}",
                            entry.Name,
                            e
                        );

                        await ToastService.Error(
                            "An error occured while downloading item",
                            entry.Name
                        );

                        failed++;
                    }
                }

                await ToastService.Info(
                    "File download completed",
                    $"Successful: {succeeded} - Failed: {failed}"
                );
            };
        });
    }

    private async Task DownloadFile(
        string path,
        string name,
        long size,
        FileDownloadToast toast,
        IFsAccess access,
        IFileManager fileManager
    )
    {
        await FileTransferService.Download(
            name,
            fileManager.TransferChunkSize,
            size,
            async id =>
            {
                return await access.DownloadChunk(
                    path,
                    id,
                    fileManager.TransferChunkSize
                );
            },
            new Progress<int>(async percent => { await toast.UpdateStatus(name, percent); })
        );
    }
}