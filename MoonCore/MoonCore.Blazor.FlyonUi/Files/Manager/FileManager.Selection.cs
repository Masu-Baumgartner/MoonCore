using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private readonly List<FsEntry> SelectedEntries = new();
    
    private async Task OnSelectionChanged()
     => await InvokeAsync(StateHasChanged);

    private async Task RunSelectionOperationAsync(IMultiFsOperation operation)
    {
        if(SelectedEntries.Count == 0)
            return;

        var workingDir = new string(CurrentPath);
        var files = SelectedEntries.ToArray();

        await operation.ExecuteAsync(workingDir, files, FsAccess, this);
    }
}