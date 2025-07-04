using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private FileEntry[] SelectedEntries = [];
    
    private async Task OnSelectionChanged(FileEntry[] entries)
    {
        SelectedEntries = entries;
        await InvokeAsync(StateHasChanged);
    }

    private async Task RunSelectionOperation(IFileOperation operation)
    {
        if(SelectedEntries.Length == 0)
            return;

        var workingDir = new string(CurrentPath);
        var files = SelectedEntries.ToArray();

        await operation.Execute(workingDir, files, FileAccess, this);
    }
}