using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Grid;

public partial class DataGrid<TGridItem>
{
    /// <summary>
    /// Toggle to enable the filtering / searching component
    /// </summary>
    [Parameter] public bool EnableFiltering { get; set; } = false;
    
    /// <summary>
    /// Toggle to enable the searching/filtering of the data source in realtime to the user input
    /// </summary>
    [Parameter] public bool EnableLiveFiltering { get; set; } = false;

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