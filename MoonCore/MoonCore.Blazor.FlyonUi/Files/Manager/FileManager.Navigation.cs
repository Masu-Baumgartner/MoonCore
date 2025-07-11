using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    public string CurrentPath { get; set; } = "/";
    
    private async Task<FsEntry[]> Load()
    {
        var items = await FsAccess.List(CurrentPath);

        return items
            .OrderByDescending(x => x.IsFolder)
            .ThenBy(x => x.Name)
            .ToArray();
    }
    
    private async Task OnClick(FsEntry fsEntry)
    {
        if (fsEntry.IsFolder)
        {
            CurrentPath = UnixPath.Combine(CurrentPath, fsEntry.Name);

            await FileView.Refresh();
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async Task GoUp()
    {
        CurrentPath = UnixPath.GetFullPath(
            UnixPath.Combine(CurrentPath, "..")
        );

        await FileView.Refresh();
    }
    
    private async Task SetPath(string path)
    {
        CurrentPath = path;

        await FileView.Refresh();
        await InvokeAsync(StateHasChanged);
    }

    private string CustomClickLink(FsEntry arg) => $"/files?path=/{arg.Name}";
}