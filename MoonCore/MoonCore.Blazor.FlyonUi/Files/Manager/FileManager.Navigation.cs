using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    public string CurrentPath { get; set; } = "/";
    
    private async Task<IEnumerable<FsEntry>> LoadAsync()
    {
        var items = await FsAccess.ListAsync(CurrentPath);

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

            await FileView.RefreshAsync();
            await InvokeAsync(StateHasChanged);
        }
        else
            await OpenAsync(fsEntry);
    }
    
    private async Task GoUpAsync()
    {
        CurrentPath = UnixPath.GetFullPath(
            UnixPath.Combine(CurrentPath, "..")
        );

        await FileView.RefreshAsync();
    }
    
    private async Task SetPathAsync(string path)
    {
        CurrentPath = path;

        await FileView.RefreshAsync();
        await InvokeAsync(StateHasChanged);
    }

    private string CustomClickLink(FsEntry arg) => $"/files?path=/{arg.Name}";
}