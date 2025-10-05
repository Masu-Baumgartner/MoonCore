using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MoonCore.Blazor.FlyonUi.Common;
using MoonCore.Blazor.FlyonUi.Grid.Columns;
using MoonCore.Blazor.FlyonUi.Grid.Rows;
using MoonCore.Blazor.FlyonUi.Grid.ToolbarItems;
using MoonCore.Common;
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
    
    /// <summary>
    /// Configure the data grid here
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Callback to set advanced options
    /// </summary>
    [Parameter] public Action<DataGridOptions<TGridItem>>? OnConfigure { get; set; }

    // Configuration
    private readonly List<ColumnBase<TGridItem>> CollectedColumns = new();
    private readonly List<RowBase<TGridItem>> CollectedRows = new();
    private readonly List<ToolbarItemBase<TGridItem>> CollectedToolbarItems = new();
    private ColumnBase<TGridItem>[] Columns;
    private RowBase<TGridItem>[] Rows;
    private ToolbarItemBase<TGridItem>[] ToolbarItems;

    private DataGridOptions<TGridItem> Options = new();

    public ColumnBase<TGridItem>? CurrentSortColumn { get; private set; }

    // States
    private bool IsInitialized = false;
    private bool IsLoadCompleted = false;
    private bool IsLoadFailed = false;

    private int StartIndex;
    private int Count;

    // Cache & UI Storage
    private Exception LoadException;
    private RenderFragment HeaderRender;
    private RenderFragment RowsRender;
    private RenderFragment ToolbarItemRender;
    private RenderFragment<TGridItem> CellsRender;

    /// <summary>
    /// Items source to retrieve the content from
    /// </summary>
    [Parameter]
    public required ItemSource<TGridItem> ItemSource { get; set; }

    public TGridItem[] Items { get; private set; } = [];
    public int TotalCount { get; private set; } = 0;

    protected override void OnInitialized()
    {
        Options.Sorting.Enable = ItemSource.IsSortable;
        Options.Filtering.Enable = ItemSource.IsFilterable;
        Options.Navigation.EnablePagination = ItemSource.IsPageable;

        OnConfigure?.Invoke(Options);

        // Set initial values
        StartIndex = Options.Navigation.StartIndex;
        Count = Options.Navigation.Count;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        Columns = CollectedColumns
            .OrderBy(x => x.Order)
            .ToArray();

        Rows = CollectedRows
            .OrderBy(x => x.Order)
            .ToArray();

        ToolbarItems = CollectedToolbarItems
            .OrderBy(x => x.Order)
            .ToArray();

        HeaderRender = RenderAndCombineHeader;
        RowsRender = RenderAndCombineRows;
        ToolbarItemRender = RenderAndCombineToolbarItems;
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
            SortOption? sortOption;

            if (!Options.Sorting.Enable || CurrentSortColumn == null ||
                string.IsNullOrWhiteSpace(CurrentSortColumn.SortingName))
                sortOption = null;
            else
            {
                sortOption = new SortOption(
                    CurrentSortColumn.SortingName!,
                    CurrentSortColumn.SortingDirection.GetValueOrDefault(SortDirection.Ascending)
                );
            }

            var items = await ItemSource.QueryAsync(StartIndex, Count, FilterInput, sortOption);

            // Save into cache
            Items = items.ToArray();

            // For remote data we provide the CountedData<T> type, which contains an additional property TotalCount
            // required for successfully paginate through the provided item source, so we check it here
            TotalCount = items is CountedData<TGridItem> countedData ? countedData.TotalCount : Items.Length;

            // Invoke column items changed event
            foreach (var column in Columns)
                await column.OnItemsChangedAsync();

            // Invoke rows items changed event
            foreach (var row in Rows)
                await row.OnItemsChangedAsync();

            // Invoke toolbar item items changed event
            foreach (var toolbarItem in ToolbarItems)
                await toolbarItem.OnItemsChangedAsync();

            IsLoadFailed = false;
        }
        catch (Exception e)
        {
            if (e is HttpApiException { Status: 401 })
                throw;

            IsLoadFailed = true;
            LoadException = e;

            Logger.LogError(e, "An unhandled error occured while loading data from ItemSource");
        }

        // Update the loading state
        IsLoadCompleted = true;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Triggers a rerender of the data grid
    /// </summary>
    public async Task NotifyStateChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    internal void AddColumn(ColumnBase<TGridItem> column)
        => CollectedColumns.Add(column);

    internal void AddRow(RowBase<TGridItem> row)
        => CollectedRows.Add(row);

    internal void AddToolbarItem(ToolbarItemBase<TGridItem> toolbarItem)
        => CollectedToolbarItems.Add(toolbarItem);

    /// <summary>
    /// Set the sorting of the provided column 
    /// </summary>
    /// <param name="column">Column to sort. Provide <b>null</b> to reset sorting</param>
    /// <param name="state">Sort direction for the column sorting. If you provided <b>null</b> in <see cref="column"/> you can provide <b>SortState.None</b></param>
    public async Task SetColumnSortingAsync(ColumnBase<TGridItem>? column, SortDirection? state)
    {
        // Reset current column if it has changed
        if (CurrentSortColumn != null && CurrentSortColumn != column)
            CurrentSortColumn.SortingDirection = null;

        CurrentSortColumn = column;

        if (CurrentSortColumn != null)
            CurrentSortColumn.SortingDirection = state;

        await RefreshAsync();
    }
}