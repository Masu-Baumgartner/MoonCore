﻿namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    private FileList FileList;
    private string CurrentPath = "/";
    
    private async Task OnPathBreadcrumbClick(string path)
    {
        CurrentPath = await FileList.Navigate(path);
        await InvokeAsync(StateHasChanged);
    }
}