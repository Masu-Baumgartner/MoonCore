using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.FlyonUi.Grid.Columns;
using MoonCore.Exceptions;

namespace MoonCore.Blazor.FlyonUi.Grid;

/// <summary>
/// Dynamic configurable grid which supports pagination, filtering and sorting
/// </summary>
/// <typeparam name="TGridItem">Generic type which should be visualized</typeparam>
[CascadingTypeParameter(nameof(TGridItem))]
public partial class DataGrid<TGridItem>
{
    // Parameters
    [Parameter] public RenderFragment ChildContent { get; set; }
    
    // Configuration
    private readonly List<ColumnBase<TGridItem>> CollectedColumns = new();
    private ColumnBase<TGridItem>[] Columns;
    private ColumnBase<TGridItem>? CurrentSortColumn;
    
    // States
    private bool IsInitialized = false;
    private bool IsLoadCompleted = false;
    private bool IsLoadFailed = false;

    // Cache & UI Storage
    private Exception LoadException;
    private RenderFragment HeaderRender;
    private RenderFragment<TGridItem> CellsRender;
    
    // Item Loading
    
    /// <summary>
    /// StartIndex to search in the data source
    /// </summary>
    [Parameter] public int StartIndex { get; set; } = 0;
    
    /// <summary>
    /// Amount of items to retrieve from the data source
    /// </summary>
    [Parameter] public int Count { get; set; } = 15;
    
    /// <summary>
    /// Data source to retrieve the items from
    /// </summary>
    [Parameter] public Func<DataGridItemRequest, Task<DataGridItemResult<TGridItem>>> ItemsProvider { get; set; }

    public TGridItem[] Items { get; private set; } = [];
    public int TotalCount { get; private set; } = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(!firstRender)
            return;

        Columns = CollectedColumns
            .OrderBy(x => x.Order)
            .ToArray();

        HeaderRender = RenderAndCombineHeader;
        CellsRender = item => builder => RenderAndCombineCells(builder, item);
        
        // Set init state
        IsInitialized = true;
        await InvokeAsync(StateHasChanged);

        // Load data
        await RefreshAsync();
    }

    /// <summary>
    /// Refreshes the grid contents from the data source
    /// </summary>
    /// <param name="silent">Whether to refresh the items without showing the loading screen</param>
    public async Task RefreshAsync(bool silent = false)
    {
        if (!silent)
        {
            // Update state if not explicit muted
            IsLoadCompleted = false;
            await InvokeAsync(StateHasChanged);
        }
        
        // Invoke the ItemProvider and update the local cache
        try
        {
            // Call provider
            var result = await ItemsProvider.Invoke(
                new DataGridItemRequest()
                {
                    Count = Count,
                    StartIndex = StartIndex,
                    SortColumn = CurrentSortColumn?.SortName ?? null,
                    SortDirection = CurrentSortColumn?.SortState ?? SortState.None,
                    Filter = FilterInput
                }
            );

            // Save into cache
            Items = result.Items.ToArray();
            TotalCount = result.TotalCount;
            
            // Invoke column items changed event
            foreach (var column in Columns)
                await column.OnItemsChangedAsync();
            
            IsLoadFailed = false;
        }
        catch (Exception e)
        {
            if (e is HttpApiException { Status: 401 })
                throw;
            
            IsLoadFailed = true;
            LoadException = e;
        }

        // Update the loading state
        IsLoadCompleted = true;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Triggers a rerender of the data grid
    /// </summary>
    public async Task NotifyStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    internal void AddColumn(ColumnBase<TGridItem> column)
        => CollectedColumns.Add(column);

    /// <summary>
    /// Set the sorting of the provided column 
    /// </summary>
    /// <param name="column">Column to sort. Provide <b>null</b> to reset sorting</param>
    /// <param name="state">Sort direction for the column sorting. If you provided <b>null</b> in <see cref="column"/> you can provide <b>SortState.None</b></param>
    public async Task SetColumnSorting(ColumnBase<TGridItem>? column, SortState state)
    {
        // Reset current column if it has changed
        if (CurrentSortColumn != null && CurrentSortColumn != column)
            CurrentSortColumn.SortState = SortState.None;

        CurrentSortColumn = column;

        if (CurrentSortColumn != null)
            CurrentSortColumn.SortState = state;

        await RefreshAsync();
    }
}