namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    private FileList FileList;
    private string CurrentPath = "/";
    
    private async Task OnPathBreadcrumbClick(string path)
    {
        await SetAllSelection(false);
        CurrentPath = await FileList.Navigate(path);
    }
}