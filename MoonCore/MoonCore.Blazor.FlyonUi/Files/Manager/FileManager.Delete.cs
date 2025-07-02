using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    public async Task DeleteViaContext()
    {
        if(ContextEntry == null)
            return;
        
        if (SelectedEntries.Length != 0 && SelectedEntries.Contains(ContextEntry)) // Handle context menu on selection
            await Delete(SelectedEntries);
        else
            await Delete([ContextEntry]);
    }
    
    public async Task Delete(FileEntry[] entries)
    {
        // We need this as to delete can run while the file manager is used to navigate elsewhere
        var currentPwd = new string(CurrentPath);

        var content = "Do you really want to delete: ";
        content += string.Join(", ", entries.Take(4).Select(x => x.Name));

        if (entries.Length > 4)
            content += $", and {entries.Length - 4} more";

        await AlertService.ConfirmDanger(
            $"You you really want to delete {entries.Length} item(s)",
            content,
            async () =>
            {
                await ToastService.Progress(
                    $"Deleing {entries.Length} item(s)",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;
                
                        foreach (var entry in entries)
                        {
                            await toast.UpdateText(entry.Name);
                            
                            try
                            {
                                await FileAccess.Delete(
                                    UnixPath.Combine(
                                        currentPwd,
                                        entry.Name
                                    )
                                );

                                successfully++;
                            }
                            catch (HttpApiException e)
                            {
                                await ToastService.Error(
                                    entry.Name,
                                    e.Title
                                );
                            }
                        }

                        await ToastService.Success($"Successfully deleted {successfully} item(s)");

                        await FileView.Refresh();
                    }
                );
            }
        );
    }
}