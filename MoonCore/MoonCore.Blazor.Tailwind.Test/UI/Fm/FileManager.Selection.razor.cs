using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Tailwind.Toasts.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager : ComponentBase
{
    private FileSystemEntry[] SelectedEntries = [];

    private async Task OnSelectionChanged(FileSystemEntry[] entries)
    {
        SelectedEntries = entries;
        await InvokeAsync(StateHasChanged);
    }

    private async Task DeleteSelection()
    {
        async Task DeleteSelectedItems(ProgressToast toast)
        {
            var entries = SelectedEntries.ToArray();
            var currentPath = CurrentPath;
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
            await FileList.Refresh();
        }

        await AlertService.ConfirmDanger(
            "Deleting multiple items",
            $"Do you really want to delete {SelectedEntries.Length} item(s)",
            async () =>
            {
                await ToastService.Progress(
                    $"Deleting {SelectedEntries.Length} items",
                    "",
                    DeleteSelectedItems
                );
            }
        );
    }
}