using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private FsEntry[] SelectedEntries = [];
    
    private async Task OnSelectionChanged(FsEntry[] entries)
    {
        SelectedEntries = entries;
        await InvokeAsync(StateHasChanged);
    }

    private async Task RunSelectionOperationAsync(IMultiFsOperation operation)
    {
        if(SelectedEntries.Length == 0)
            return;

        var workingDir = new string(CurrentPath);
        var files = SelectedEntries.ToArray();

        await operation.ExecuteAsync(workingDir, files, FsAccess, this);
    }
}