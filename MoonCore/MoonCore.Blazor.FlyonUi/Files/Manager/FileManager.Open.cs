using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private RenderFragment? OpenWindow;
    
    private async Task Open(FsEntry entry)
    {
        var openOperation = Options
            .OpenOperations
            .FirstOrDefault(x => x.Filter.Invoke(entry));

        if (openOperation == null)
        {
            await ToastService.Error("Cannot open the file: Not supported");
            return;
        }

        if (Options.OpenLimit != -1 && entry.Size > Options.OpenLimit)
        {
            await ToastService.Error("Cannot open the file: Exceeded the open limit");
            return;
        }
        
        var pwd = new string(CurrentPath);

        OpenWindow = await openOperation.Open(
            pwd,
            entry,
            FsAccess,
            this
        );

        await InvokeAsync(StateHasChanged);
    }
    
    public async Task CloseOpenScreen()
    {
        OpenWindow = null;
        await InvokeAsync(StateHasChanged);
    }
}