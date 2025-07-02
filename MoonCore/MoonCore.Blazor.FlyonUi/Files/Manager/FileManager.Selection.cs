namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private FileEntry[] SelectedEntries = [];
    
    private async Task OnSelectionChanged(FileEntry[] entries)
    {
        SelectedEntries = entries;
        await InvokeAsync(StateHasChanged);
    }
}