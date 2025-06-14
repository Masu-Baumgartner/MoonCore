using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MoonCore.Exceptions;

namespace MoonCore.Blazor.FlyonUi.DataTables;

public partial class DataTable<TItem>
{
    #region Blazor Hooks

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(!firstRender)
            return;
        
        // Build columns
        SortedColumns = Columns.OrderBy(x => x.Index).ToArray();
        SortedRows = Rows.OrderBy(x => x.Index).ToArray();

        await Refresh();
    }

    #endregion
    
    #region Configuration
    
    [Parameter] public RenderFragment? Configuration { get; set; }

    private readonly List<DataTableColumn<TItem>> Columns = new();
    private readonly List<DataTableRow<TItem>> Rows = new();

    private DataTableColumn<TItem>[] SortedColumns = [];
    private DataTableRow<TItem>[] SortedRows = [];

    internal void AddColumn(DataTableColumn<TItem> column)
        => Columns.Add(column);

    internal void AddRow(DataTableRow<TItem> row)
        => Rows.Add(row);

    #endregion

    #region Items & Loading

    [Parameter] public Func<Task<TItem[]>> ItemSource { get; set; }

    public event Func<Task>? OnRefreshed;
    public event Func<Task>? OnRefreshing;

    public bool IsLoading { get; private set; } = true;
    public TItem[] Items { get; private set; } = [];
    private Exception? LoadingException;

    public async Task Refresh(bool silent = false)
    {
        // Show loading state if not set to silent
        if (!silent)
        {
            IsLoading = true;
            await InvokeAsync(StateHasChanged);
        }

        // Reset variables
        LoadingException = null;
        
        // Notify event listeners
        if (OnRefreshing != null)
            await OnRefreshing.Invoke();

        // Load items
        try
        {
            Items = await ItemSource.Invoke();
        }
        catch (Exception e)
        {
            // Dont handle unauthenticated errors
            if (e is HttpApiException httpApiException && httpApiException.Status == 401)
                throw;
            
            Logger.LogError("An error occured while loading items from source: {e}", e);
            LoadingException = e;
        }
        finally
        {
            IsLoading = false;
            await InvokeAsync(StateHasChanged);

            if (OnRefreshed != null)
                await OnRefreshed.Invoke();
        }
    }

    public async Task NotifyStateHasChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Customization

    [Parameter] public RenderFragment? LoadingIndicator { get; set; }
    [Parameter] public RenderFragment<Exception>? ExceptionIndicator { get; set; }
    [Parameter] public RenderFragment? NoItemsIndicator { get; set; }

    [Parameter] public string ContainerCss { get; set; } = "w-full overflow-x-auto";
    [Parameter] public string RowCss { get; set; }
    
    [Parameter] public RenderFragment? Header { get; set; }
    [Parameter] public RenderFragment? Footer { get; set; }

    #endregion

    #region Context Menu Handling

    [Parameter] public Func<MouseEventArgs, TItem, Task>? OnContextMenu { get; set; }

    private async Task InvokeContextMenu(MouseEventArgs args, TItem item)
    {
        if(OnContextMenu == null)
            return;

        await OnContextMenu.Invoke(args, item);
    }

    #endregion
}