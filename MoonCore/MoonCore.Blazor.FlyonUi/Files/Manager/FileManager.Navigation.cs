using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    public string CurrentPath { get; set; } = "/";
    
    private async Task<FileEntry[]> Load()
    {
        var items = await FileAccess.List(CurrentPath);

        return items
            .OrderByDescending(x => x.IsFolder)
            .ThenBy(x => x.Name)
            .ToArray();
    }
    
    private async Task OnClick(FileEntry fileEntry)
    {
        if (fileEntry.IsFolder)
        {
            CurrentPath = UnixPath.Combine(CurrentPath, fileEntry.Name);

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

    private string CustomClickLink(FileEntry arg) => $"/files?path=/{arg.Name}";
}