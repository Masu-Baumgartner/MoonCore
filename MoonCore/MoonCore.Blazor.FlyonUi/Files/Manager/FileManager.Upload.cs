using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
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
                        await ToastService.Warning($"Unable to upload file as it exceeds the file upload limit: {file.Name}");
                        continue;
                    }

                    await toast.UpdateStatus(file.Name, 0);

                    try
                    {
                        await using var dataStream = file.OpenReadStream(
                            UploadLimit == -1
                                ? file.Size
                                : UploadLimit
                        );

                        await FileAccess.Upload(
                            UnixPath.Combine(pwdAtUpload, file.Name),
                            dataStream,
                            async percent =>
                            {
                                await toast.UpdateStatus(file.Name, percent);
                            }
                        );

                        succeeded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            "An unhandled error occured while uploading file {name}: {e}",
                            file.Name,
                            e
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
            
                    if(item == null)
                        break;

                    if (item.ShouldSkipToNext)
                    {
                        await DropHandlerService.PopItem();
                        continue;
                    }
            
                    if (UploadLimit != -1 && item.Stream.Length > UploadLimit)
                    {
                        await ToastService.Warning($"Unable to upload file as it exceeds the file upload limit: {UnixPath.GetFileName(item.Path)}");
                        await DropHandlerService.PopItem();
                        continue;
                    }

                    var fileName = UnixPath.GetFileName(item.Path);
                    
                    await toast.UpdateStatus(fileName, 0);

                    try
                    {
                        await using var dataStream = await item.Stream.OpenReadStreamAsync(
                            UploadLimit == -1
                                ? item.Stream.Length
                                : UploadLimit
                        );

                        await FileAccess.Upload(
                            UnixPath.Combine(pwdAtUpload, item.Path),
                            dataStream,
                            async percent => { await toast.UpdateStatus(fileName, percent); }
                        );

                        succeeded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            "An unhandled error occured while uploading file {name}: {e}",
                            fileName,
                            e
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
}