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
        if (!firstRender)
            return;

        // Build columns
        SortedColumns = Columns.OrderBy(x => x.Index).ToArray();
        SortedRows = Rows.OrderBy(x => x.Index).ToArray();

        await RefreshAsync();
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Configuration components need to be placed in here
    /// </summary>
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

    /// <summary>
    /// Callback to fetch the items which will be shown
    /// </summary>
    [Parameter] public Func<Task<TItem[]>> ItemSource { get; set; }

    /// <summary>
    /// Event which will be called when the refreshing is done
    /// </summary>
    public event Func<Task>? OnRefreshed;
    
    /// <summary>
    /// Event which will be called when the table is currently refreshing
    /// </summary>
    public event Func<Task>? OnRefreshing;

    /// <summary>
    /// Indicates is the table is currently loading
    /// </summary>
    public bool IsLoading { get; private set; } = true;
    
    /// <summary>
    /// Currently loaded items can be accessed using this property
    /// </summary>
    public TItem[] Items { get; private set; } = [];
    
    private Exception? LoadingException;

    /// <summary>
    /// Refreshes the table, refetching the items from the source
    /// </summary>
    /// <param name="silent">Is true, no loading animation will be shown</param>
    public async Task RefreshAsync(bool silent = false)
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

            Logger.LogError(e, "An error occured while loading items from source");
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

    /// <summary>
    /// Proxy function to invoke StateHasChanged in the data table
    /// </summary>
    public async Task NotifyStateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Customization

    /// <summary>
    /// <b>Optional:</b> Template to replace the default loading indicator
    /// </summary>
    [Parameter] public RenderFragment? LoadingIndicator { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Template to replace the exception indicator
    /// </summary>
    [Parameter] public RenderFragment<Exception>? ExceptionIndicator { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Template to replace the no items indicator
    /// </summary>
    [Parameter] public RenderFragment? NoItemsIndicator { get; set; }

    /// <summary>
    /// <b>Optional:</b> CSS Classes for the container the table is inside. Default value is: <b>w-full overflow-x-auto</b>
    /// </summary>
    [Parameter] public string ContainerCss { get; set; } = "w-full overflow-x-auto";
    
    /// <summary>
    /// <b>Optional:</b> CSS Classes for the rows
    /// </summary>
    [Parameter] public string RowCss { get; set; }

    /// <summary>
    /// <b>Optional:</b> Template to attach to the table as a header
    /// </summary>
    [Parameter] public RenderFragment? Header { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Template to attach zo the table as a footer
    /// </summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    #endregion

    private RenderFragment CreateRow(TItem item) => __builder =>
    {
        var handlerSeq = 0;

        // Base element

        __builder.OpenElement(0, "tr");

        __builder.AddAttribute(1, "css", RowCss);

        handlerSeq += 2;

        // Optional event handlers

        if (OnClick != null)
        {
            __builder.AddAttribute(
                handlerSeq,
                "onclick",
                EventCallback.Factory.Create(this, () => OnClick.Invoke(item))
            );
            
            __builder.AddAttribute(handlerSeq + 1, "style", "cursor: pointer");

            handlerSeq += 2;
        }

        if (OnContextMenu != null)
        {
            __builder.AddAttribute(
                handlerSeq,
                "oncontextmenu",
                EventCallback.Factory.Create(this, (MouseEventArgs args) => OnContextMenu.Invoke(args, item))
            );

            __builder.AddEventPreventDefaultAttribute(handlerSeq + 1, "oncontextmenu", true);

            handlerSeq += 2;
        }

        // Built content

        foreach (var column in SortedColumns)
        {
            __builder.OpenElement(handlerSeq, "td");
            __builder.AddAttribute(handlerSeq + 1, "scope", "row");
            __builder.AddAttribute(handlerSeq + 2, "class", column.ColumnCss);
            handlerSeq += 3;

            if (column.ColumnTemplate == null)
            {
                if (column.Field != null)
                {
                    var val = column.Field.Invoke(item);
                    __builder.AddContent(handlerSeq, val?.ToString() ?? "null");

                    handlerSeq++;
                }
            }
            else
            {
                __builder.AddContent(handlerSeq, column.ColumnTemplate.Invoke(item));
                handlerSeq++;
            }

            __builder.CloseElement();
        }

        // Finalize

        __builder.CloseElement();
    };

    #region Context Menu Handling

    /// <summary>
    /// <b>Optional:</b> Event callback for context menu actions on the row of a specific item. If none is set, no context menu events will be subscribed to from the DOM
    /// </summary>
    [Parameter] public Func<MouseEventArgs, TItem, Task>? OnContextMenu { get; set; }

    private async Task InvokeContextMenuAsync(MouseEventArgs args, TItem item)
    {
        if (OnContextMenu == null)
            return;

        await OnContextMenu.Invoke(args, item);
    }

    #endregion

    #region On Click Handling

    /// <summary>
    /// <b>Optional:</b> Event callback for the onclick event on the row of a specific item. If none is set, no onclick events will be subscribed to from the DOM
    /// </summary>
    [Parameter] public Func<TItem, Task>? OnClick { get; set; }

    #endregion
}