using Microsoft.AspNetCore.Components.Web;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private FsEntry? ContextEntry;

    private async Task RunSingleContextOperation(ISingleFsOperation operation)
    {
        if (ContextEntry == null) // Handle possible invalid calls
            return;
        
        var workingDir = new string(CurrentPath);

        await operation.Execute(
            workingDir,
            ContextEntry,
            FsAccess,
            this
        );
    }

    private async Task RunMultiContextOperation(IMultiFsOperation operation)
    {
        if (ContextEntry == null) // Handle possible invalid calls
            return;
        
        FsEntry[] files;
        var workingDir = new string(CurrentPath);

        // When the operation can be run at multiple entries and the current context entry is within a possible selection
        // we want to use the whole selection in order to make the selection "context menu clickable"
        if (SelectedEntries.Length > 0 && SelectedEntries.Contains(ContextEntry))
            files = SelectedEntries.ToArray();
        else
            files = [ContextEntry];

        await operation.Execute(
            workingDir,
            files,
            FsAccess,
            this
        );
    }

    private async Task OnContextMenu(MouseEventArgs mouseEvent, FsEntry entry)
    {
        ContextEntry = entry;
        await ContextMenu.Trigger(mouseEvent);
    }

    private async Task OnContextMenu(double x, double y, FsEntry entry)
    {
        ContextEntry = entry;
        await ContextMenu.Trigger(x, y);
    }
}