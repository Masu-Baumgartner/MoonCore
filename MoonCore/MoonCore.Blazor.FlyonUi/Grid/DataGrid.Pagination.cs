using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Grid;

public partial class DataGrid<TGridItem>
{
    /// <summary>
    /// Toggle to enable the pagination component on the table
    /// </summary>
    [Parameter] public bool EnablePagination { get; set; } = false;
    
    /// <summary>
    /// Page sizes to offer the user to select from
    /// </summary>
    [Parameter] public int[] PageSizes { get; set; } = [15, 30, 100];
    
    public async Task NavigateAsync(int diff)
    {
        StartIndex += diff;

        if (StartIndex < 0)
            StartIndex = 0;

        if (StartIndex > TotalCount)
            StartIndex = TotalCount - Count;

        await RefreshAsync();
    }

    public async Task NavigateStartAsync()
    {
        StartIndex = 0;
        await RefreshAsync();
    }
    
    public async Task NavigateEndAsync()
    {
        StartIndex = TotalCount - Count;
        await RefreshAsync();
    }

    private async Task UpdatePaginationAsync(ChangeEventArgs args)
    {
        if(args.Value == null || !int.TryParse(args.Value.ToString(), out var pageSize))
            return;
        
        Count = pageSize;
        await RefreshAsync();
    }
}