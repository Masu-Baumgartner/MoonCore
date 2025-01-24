namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    private FileView FileView;
    private string CurrentPath = "/";
    
    private async Task OnNavigated(string newPath)
    {
        CurrentPath = newPath;
        await InvokeAsync(StateHasChanged);
    }
}