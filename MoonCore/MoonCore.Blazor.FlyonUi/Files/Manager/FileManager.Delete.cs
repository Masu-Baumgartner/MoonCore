using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private async Task DeleteViaContext()
    {
        if (ContextEntry == null)
            return;

        if (SelectedEntries.Length != 0 && SelectedEntries.Contains(ContextEntry)) // Handle context menu on selection
            await Delete(SelectedEntries);
        else
            await Delete([ContextEntry]);
    }

    private Task DeleteSelection()
        => Delete(SelectedEntries);

    private async Task Delete(FileEntry[] entries)
    {
        // We need this as to delete can run while the file manager is used to navigate elsewhere
        var currentPwd = new string(CurrentPath);

        
    }
}