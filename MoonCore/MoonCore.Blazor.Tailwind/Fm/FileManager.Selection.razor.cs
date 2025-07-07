using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MoonCore.Blazor.Tailwind.Toasts.Components;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager : ComponentBase
{
    private FileSystemEntry[] SelectedEntries = [];

    private async Task UpdateSelection(FileSystemEntry[] entries)
    {
        SelectedEntries = entries;
        await InvokeAsync(StateHasChanged);
    }
    
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
                    await FileSystemProvider.Delete(UnixPath.Combine(currentPath, entry.Name));

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

    private async Task MoveSelection()
    {
        await ModalService.Launch<LocationSelectModal>(size: "max-w-2xl", onConfigure: parameters =>
        {
            parameters.Add("OnSubmit", async (string path) =>
            {
                try
                {
                    var entries = SelectedEntries.ToArray();

                    foreach (var entry in entries)
                    {
                        await FileSystemProvider.Move(
                            UnixPath.Combine(CurrentPath, entry.Name),
                            UnixPath.Combine(path, entry.Name)
                        );
                    }

                    await ToastService.Success("Successfully moved item(s)");
                }
                finally
                {
                    // Reset state
                    await FileList.Refresh();
                }
            });

            parameters.Add("FileSystemProvider", FileSystemProvider);
        });
    }
}