using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Toasts;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private Task InitializeUpload()
    {
        DropHandlerService.OnDropped += HandleFileDrop;

        return Task.CompletedTask;
    }

    private async Task LateInitializeUpload()
    {
        await DropHandlerService.Enable();
    }

    private async Task HandleFileUpload(IBrowserFile[] files)
    {
        await ToastService.Launch<FileUploadToast>(parameters =>
        {
            parameters["Callback"] = async (FileUploadToast toast) =>
            {
                // We need this as the upload can run while the file manager is used to navigate elsewhere
                var pwdAtUpload = new string(CurrentPath);

                var failed = 0;
                var succeeded = 0;

                foreach (var file in files)
                {
                    if (UploadLimit != -1 && file.Size > UploadLimit)
                    {
                        await ToastService.Warning(
                            $"Unable to upload file as it exceeds the file upload limit: {file.Name}");
                        continue;
                    }

                    await toast.SetProgress(file.Name, 0);

                    try
                    {
                        await using var dataStream = file.OpenReadStream(
                            UploadLimit == -1
                                ? file.Size
                                : UploadLimit
                        );

                        var path = UnixPath.Combine(pwdAtUpload, file.Name);

/*
                        await FileTransferService.Upload(
                            dataStream,
                            TransferChunkSize,
                            async (id, content) =>
                            {
                                await FsAccess.UploadChunk(
                                    UnixPath.Combine(pwdAtUpload, file.Name),
                                    id,
                                    TransferChunkSize,
                                    dataStream.Length,
                                    content
                                );
                            },
                            new Progress<int>(async percent => { await toast.UpdateStatus(file.Name, percent); })
                        );*/

                        if (await Upload(path, dataStream, toast))
                            succeeded++;
                        else
                            failed++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            e,
                            "An unhandled error occured while uploading file {name}",
                            file.Name
                        );

                        await ToastService.Error(
                            "An error occured while uploading file",
                            file.Name
                        );

                        failed++;
                    }
                }

                await ToastService.Info(
                    "File upload completed",
                    $"Successful: {succeeded} - Failed: {failed}"
                );

                await FileView.Refresh();
            };
        });
    }

    private async Task HandleFileDrop()
    {
        await ToastService.Launch<FileUploadToast>(parameters =>
        {
            parameters["Callback"] = async (FileUploadToast toast) =>
            {
                // We need this as the upload can run while the file manager is used to navigate elsewhere
                var pwdAtUpload = new string(CurrentPath);

                var failed = 0;
                var succeeded = 0;

                var isFirstPeek = true;

                do
                {
                    var item = await DropHandlerService.PeekItem();

                    if (isFirstPeek)
                    {
                        if (item == null)
                        {
                            await ToastService.Error(
                                "Unable to handle dropped files and/or folders. Please try again"
                            );

                            return;
                        }
                        else
                            isFirstPeek = false;
                    }

                    if (item == null)
                        break;

                    if (item.ShouldSkipToNext)
                    {
                        await DropHandlerService.PopItem();
                        continue;
                    }

                    if (UploadLimit != -1 && item.Stream.Length > UploadLimit)
                    {
                        await ToastService.Warning(
                            $"Unable to upload file as it exceeds the file upload limit: {UnixPath.GetFileName(item.Path)}");
                        await DropHandlerService.PopItem();
                        continue;
                    }

                    var fileName = UnixPath.GetFileName(item.Path);

                    await toast.SetProgress(fileName, 0);

                    try
                    {
                        await using var dataStream = await item.Stream.OpenReadStreamAsync(
                            UploadLimit == -1
                                ? item.Stream.Length
                                : UploadLimit
                        );

                        await FileTransferService.Upload(
                            dataStream,
                            TransferChunkSize,
                            async (id, content) =>
                            {
                                await FsAccess.UploadChunk(
                                    UnixPath.Combine(pwdAtUpload, item.Path),
                                    id,
                                    TransferChunkSize,
                                    dataStream.Length,
                                    content
                                );
                            },
                            new Progress<int>(async percent => { await toast.SetProgress(fileName, percent); })
                        );

                        succeeded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            e,
                            "An unhandled error occured while uploading file {name}",
                            fileName
                        );

                        await ToastService.Error(
                            "An error occured while uploading file",
                            fileName
                        );

                        failed++;
                    }
                    finally
                    {
                        await DropHandlerService.PopItem();
                    }
                } while (true);

                await ToastService.Info(
                    "File upload completed",
                    $"Successful: {succeeded} - Failed: {failed}"
                );

                await FileView.Refresh();
            };
        });
    }

    private async Task<bool> Upload(string path, Stream stream, FileUploadToast toast)
    {
        var name = UnixPath.GetFileName(path);
        
        var transferLimit = ByteConverter.FromMegaBytes(20).Bytes;

        var combineAccess = FsAccess as ICombineAccess;

        if (combineAccess == null || stream.Length < transferLimit) // No chunked upload possible or required
        {
            if (stream.Length > transferLimit)
            {
                await ToastService.Error(
                    "File upload limit exceeded",
                    $"Unable to upload {name}: Exceeds limit of {Formatter.FormatSize(UploadLimit)}"
                );

                return false;
            }

            await FsAccess.Write(path, stream);

            return true;
        }
        else
        {
            Logger.LogInformation("Start");
            
            var workerCount = 1;
            var tasks = new List<Task>();
            var locker = new SemaphoreSlim(1);
            var id = Random.Shared.Next(0, 1024);
            var uploadDir = Path.Combine("/", $".upload-{id}");
            var counter = 0;
            double lastLoggedPercent = -1;

            try
            {
                for (var i = 0; i < workerCount; i++)
                {
                    var threadIndex = i; // For logging

                    var task = Task.Run(async () =>
                    {
                        while (true)
                        {
                            var ms = new MemoryStream();
                        
                            int currentCount;
                            var totalLength = stream.Length;

                            // Safely copy from stream
                            await locker.WaitAsync();
                            try
                            {
                                if (stream.Position >= stream.Length)
                                {
                                    break;
                                }

                                await CopyStreamPart(stream, ms, transferLimit);

                                // Skip empty parts
                                if (ms.Length == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    Logger.LogInformation("Part: {size}", ms.Length);
                                }

                                currentCount = counter++;
                                var currentPosition = stream.Position;

                                // Log progress if at least 1% more completed
                                var percent = (double)currentPosition / totalLength * 100;
                                if (percent != lastLoggedPercent)
                                {
                                    lastLoggedPercent = percent;
                                
                                    await toast.SetProgress(name, (int)Math.Round(percent));
                                
                                    Logger.LogInformation("Progress: {Percent}%", percent);
                                }
                            }
                            finally
                            {
                                locker.Release();
                            }

                            ms.Position = 0;
                            var partPath = UnixPath.Combine(uploadDir, $"{currentCount}.bin");

                            var uploadTries = 0;
                            const int maxUploadTries = 3;

                            while (uploadTries < maxUploadTries)
                            {
                                try
                                {
                                    await FsAccess.Write(partPath, ms);
                                    Logger.LogInformation("Thread {ThreadIndex}: Sent file to {Path}", threadIndex,
                                        partPath);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Logger.LogWarning(e, "Error uploading part {PartPath}, attempt {Attempt}", partPath,
                                        uploadTries + 1);
                                    uploadTries++;
                                    if (uploadTries >= maxUploadTries)
                                    {
                                        Logger.LogError("Failed to upload {Path} after {Tries} attempts", partPath,
                                            maxUploadTries);
                                    }
                                }
                            }
                        }
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                locker.Dispose();
            
                var paths = new List<string>();

                for (var i = 0; i < counter; i++)
                    paths.Add(UnixPath.Combine(uploadDir, $"{i}.bin"));

                await toast.SetCombining();
                await combineAccess.Combine(path, paths.ToArray());
            }
            finally
            {
                await FsAccess.Delete(uploadDir);
            }

            return true;
        }
    }

    private async Task CopyStreamPart(Stream input, Stream output, long length)
    {
        var buffer = new byte[80 * 1024]; // 80 KB buffer
        long remaining = length;

        while (remaining > 0)
        {
            int toRead = remaining > buffer.Length
                ? buffer.Length
                : (int)remaining;

            int bytesRead = await input.ReadAsync(buffer, 0, toRead).ConfigureAwait(false);
            if (bytesRead == 0)
                break; // end of input stream

            await output.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
            remaining -= bytesRead;
        }
    }
}