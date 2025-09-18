using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private bool ShowOpenWindow = false;
    private RenderFragment? OpenWindow;

    private async Task OpenAsync(FsEntry entry)
    {
        var openOperation = OpenOperations.FirstOrDefault(x =>
            x.Filter.Invoke(entry)
        );

        if (openOperation == null)
        {
            await ToastService.ErrorAsync("Cannot open the file: Not supported");
            return;
        }

        if (Options.OpenLimit != -1 && entry.Size > Options.OpenLimit)
        {
            await ToastService.ErrorAsync("Cannot open the file: Exceeded the open limit");
            return;
        }

        var pwd = new string(CurrentPath);

        OpenWindow = await openOperation.OpenAsync(
            pwd,
            entry,
            FsAccess,
            this
        );

        if (OpenWindow == null)
        {
            await ToastService.ErrorAsync("Cannot open file: Unknown error");
            return;
        }

        ShowOpenWindow = true;
        await InvokeAsync(StateHasChanged);
    }

    public async Task CloseOpenScreenAsync()
    {
        ShowOpenWindow = false;
        await InvokeAsync(StateHasChanged);
    }
}