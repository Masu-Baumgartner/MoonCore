using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Grid;

public partial class DataGrid<TGridItem>
{
    private string? FilterInput;

    private async Task FilterAsync(ChangeEventArgs args)
    {
        FilterInput = args.Value?.ToString() ?? "";
        await FilterAsync();
    }

    private async Task FilterAsync()
    {
        StartIndex = 0;
        await RefreshAsync();
    }
}