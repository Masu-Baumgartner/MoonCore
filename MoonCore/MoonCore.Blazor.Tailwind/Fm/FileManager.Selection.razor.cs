using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MoonCore.Blazor.Tailwind.Toasts.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager : ComponentBase
{
    private List<FileSystemEntry> SelectedEntries = new();

    #region Select Impl

    private async Task SetSelection(FileSystemEntry entry, bool toggle)
    {
        if (toggle)
        {
            if(!SelectedEntries.Contains(entry))
                SelectedEntries.Add(entry);
        }
        else
        {
            if (SelectedEntries.Contains(entry))
                SelectedEntries.Remove(entry);
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task SetAllSelection(bool toggle)
    {
        SelectedEntries.Clear();
        
        if(toggle)
            SelectedEntries.AddRange(FileList.LoadedEntries);

        await InvokeAsync(StateHasChanged);
    }

    #endregion

    private async Task DeleteSelection()
    {
        async Task DeleteSelectedItems(ProgressToast toast)
        {
            // We create an array here because the user could change the selection while we are working on deleting the selected files
            var entries = SelectedEntries.ToArray();
            
            // Same reason as above
            var currentPath = CurrentPath;
            
            // To keep track of things
            var deleted = 0;
            
            foreach (var entry in entries)
            {
                await toast.UpdateText(entry.Name);

                try
                {
                    await FileSystemProvider.Delete(PathBuilder.JoinPaths(currentPath, entry.Name));

                    deleted++;
                }
                catch (Exception e)
                {
                    await ToastService.Danger($"An error occured while deleting '{entry.Name}'");
                    
                    Logger.LogError("An unhandled error occured while deleting file '{name}': {e}", entry.Name, e);
                }
            }

            await ToastService.Success($"Successfully deleted {deleted} files");
            
            // Reset state
            await SetAllSelection(false);
            await FileList.Refresh();
        }

        await AlertService.ConfirmDanger(
            "Deleting multiple items",
            $"Do you really want to delete {SelectedEntries.Count} item(s)",
            async () =>
            {
                await ToastService.Progress(
                    $"Deleting {SelectedEntries.Count} items",
                    "",
                    DeleteSelectedItems
                );
            }
        );
    }
}