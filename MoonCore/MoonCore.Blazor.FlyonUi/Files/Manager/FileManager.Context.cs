using Microsoft.AspNetCore.Components.Web;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private FileEntry? ContextEntry;

    private async Task OnContextMenu(MouseEventArgs mouseEvent, FileEntry entry)
    {
        ContextEntry = entry;
        await ContextMenu.Trigger(mouseEvent);
    }

    private async Task OnContextMenu(double x, double y, FileEntry entry)
    {
        ContextEntry = entry;
        await ContextMenu.Trigger(x, y);
    }
}