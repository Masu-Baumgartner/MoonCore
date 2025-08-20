using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
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

    private async ValueTask DisposeUpload()
    {
        DropHandlerService.OnDropped -= HandleFileDrop;

        try
        {
            await DropHandlerService.Disable();
        }
        catch (JSDisconnectedException)
        {
            // Ignored
        }
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
                    if (Options.UploadLimit != -1 && file.Size > Options.UploadLimit)
                    {
                        await ToastService.Warning(
                            $"Unable to upload file as it exceeds the file upload limit: {file.Name}");
                        continue;
                    }

                    await toast.SetProgress(file.Name, 0);

                    try
                    {
                        await using var dataStream = file.OpenReadStream(
                            Options.UploadLimit == -1
                                ? file.Size
                                : Options.UploadLimit
                        );

                        var path = UnixPath.Combine(pwdAtUpload, file.Name);

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
        await ToastService.Launch<FileUploadToast>(parameters => { parameters["Callback"] = UploadFromDropzone; });
    }

    private async Task UploadFromDropzone(FileUploadToast toast)
    {
        // We need this as the upload can run while the file manager is used to navigate elsewhere
        var targetDir = new string(CurrentPath);

        // Counters
        var succeeded = 0;
        var failed = 0;

        var item = await DropHandlerService.PeekItem();

        if (item == null)
        {
            Logger.LogWarning("Unable to load first item from dropzone. Exiting early");

            await ToastService.Error("Unable to handle dropped files and/or folders. Please try again");
            return;
        }

        while (true)
        {
            item = await DropHandlerService.PeekItem();

            if (item == null)
                break;

            if (item.ShouldSkipToNext)
            {
                await DropHandlerService.PopItem();
                continue;
            }

            var fileName = UnixPath.GetFileName(item.Path);

            if (Options.UploadLimit != -1 && item.Stream.Length > Options.UploadLimit)
            {
                await ToastService.Warning($"Unable to upload file as it exceeds the file upload limit: {fileName}");
                await DropHandlerService.PopItem();
                continue;
            }

            await toast.SetProgress(fileName, 0);

            try
            {
                // Retrieve data stream
                await using var dataStream = await item.Stream.OpenReadStreamAsync(item.Stream.Length);

                if (await Upload(UnixPath.Combine(targetDir, item.Path), dataStream, toast))
                    succeeded++;
                else
                    failed++;
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
        }

        await ToastService.Info(
            "File upload completed",
            $"Successful: {succeeded} - Failed: {failed}"
        );

        await FileView.Refresh();
    }

    private async Task<bool> Upload(string path, Stream stream, FileUploadToast toast)
    {
        var name = UnixPath.GetFileName(path);

        var combineAccess = FsAccess as ICombineAccess;

        if (stream.Length < Options.WriteLimit) // If smaller than the transfer limit we won't even brother checking for chunking support
        {
            try
            {
                await FsAccess.Write(path, stream);
            }
            catch (Exception e)
            {
                Logger.LogInformation(e, "An unhandled error occured while uploading file: {name}", name);
                return false;
            }
        }
        else
        {
            // If the file is larger than the transfer limit we need to check for chunking support
            // and handle the chunked upload
            
            if (combineAccess == null) // No chunked upload available => no way to upload the file as it exceeds regular limits
            {
                await ToastService.Error(
                    "File upload limit exceeded",
                    $"Unable to upload {name}: Exceeds limit of {Formatter.FormatSize(Options.WriteLimit)}"
                );

                return false;
            }
            
            const int workerCount = 1;
            const int maxUploadTries = 3;
            var locker = new SemaphoreSlim(1);
            var tasks = new List<Task>();
            var id = Random.Shared.Next(0, 1024);
            var uploadDir = UnixPath.Combine("/", $".upload-{id}");
            var counter = 0;

            var chunkPaths = new ConcurrentList<string>();

            try
            {
                for (var i = 0; i < workerCount; i++)
                {
                    var threadIndex = i; // For logging

                    var task = Task.Run(async () =>
                    {
                        while (true)
                        {
                            using var ms = new MemoryStream();

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

                                await CopyStreamPart(stream, ms, Options.WriteLimit);

                                // Skip empty parts
                                if (ms.Length == 0)
                                    break;

                                currentCount = counter++;
                            }
                            finally
                            {
                                locker.Release();
                            }

                            ms.Position = 0;
                            var partPath = UnixPath.Combine(uploadDir, $"{currentCount}.bin");

                            chunkPaths.Add(partPath);

                            var uploadTries = 0;

                            while (uploadTries < maxUploadTries)
                            {
                                try
                                {
                                    await FsAccess.Write(partPath, ms);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    uploadTries++;

                                    Logger.LogWarning(
                                        e,
                                        "Thread {threadId}: Error uploading part {pathPart}, attempt {tryCounter}",
                                        threadIndex,
                                        partPath,
                                        uploadTries
                                    );
                                }
                            }

                            try
                            {
                                await locker.WaitAsync();

                                var currentPosition = stream.Position;

                                var percent = (int)((double)currentPosition / totalLength * 100f);
                                await toast.SetProgress(name, percent);
                            }
                            finally
                            {
                                locker.Release();
                            }
                        }
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                locker.Dispose();

                await toast.SetCombining();

                await combineAccess.Combine(path, chunkPaths.ToArray());
            }
            finally
            {
                await FsAccess.Delete(uploadDir);
            }
        }

        return true;
    }

    private async Task CopyStreamPart(Stream input, Stream output, long length)
    {
        var buffer = new byte[80 * 1024]; // 80 KB buffer
        var remaining = length;

        while (remaining > 0)
        {
            var toRead = remaining > buffer.Length
                ? buffer.Length
                : (int)remaining;

            var bytesRead = await input.ReadAsync(buffer, 0, toRead);
            if (bytesRead == 0)
                break; // end of input stream

            await output.WriteAsync(buffer, 0, bytesRead);
            remaining -= bytesRead;
        }
    }
}