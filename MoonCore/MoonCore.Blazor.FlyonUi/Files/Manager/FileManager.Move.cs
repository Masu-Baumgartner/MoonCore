using MoonCore.Blazor.FlyonUi.Files.Manager.Modals;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private async Task MoveViaContext()
    {
        if (ContextEntry == null)
            return;

        if (SelectedEntries.Length != 0 && SelectedEntries.Contains(ContextEntry)) // Handle context menu on selection
            await Move(SelectedEntries);
        else
            await Move([ContextEntry]);
    }

    private Task MoveSelection()
        => Move(SelectedEntries);

    private async Task Move(FileEntry[] entries)
    {
        // We need this as to move can run while the file manager is used to navigate elsewhere
        var currentPwd = new string(CurrentPath);

        await ModalService.Launch<MoveModal>(parameters =>
        {
            parameters["Title"] = $"Select a location to move {entries.Length} item(s) to";
            parameters["FileAccess"] = FileAccess;
            parameters["InitialPath"] = CurrentPath;
            parameters["OnSubmit"] = async (string path) =>
            {
                await ToastService.Progress(
                    $"Moving {entries.Length} items",
                    "Preparing",
                    async toast =>
                    {
                        var successfully = 0;
                        
                        foreach (var entry in entries)
                        {
                            await toast.UpdateText(entry.Name);

                            try
                            {
                                await FileAccess.Move(
                                    UnixPath.Combine(currentPwd, entry.Name),
                                    UnixPath.Combine(path, entry.Name)
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
                        
                        await ToastService.Success($"Successfully moved {successfully} item(s)");

                        await FileView.Refresh();
                    }
                );
            };
        }, "max-w-xl");
    }
}