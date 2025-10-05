using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MoonCore.Blazor.FlyonUi.Common;

namespace MoonCore.Blazor.FlyonUi.Grid.Columns;

public abstract partial class ColumnBase<TGridItem>
{
    /// <summary>
    /// Reference to the DataGrid parent
    /// </summary>
    [CascadingParameter] public DataGrid<TGridItem> Parent { get; set; }

    // Parameters
    
    /// <summary>
    /// Template for the header of the column. If <b>null</b> is provided the default header will be used
    /// </summary>
    [Parameter] public RenderFragment<ColumnBase<TGridItem>>? HeaderTemplate { get; set; }
    
    /// <summary>
    /// Order number for the column. Higher values place it further to the right
    /// </summary>
    [Parameter] public int Order { get; set; } = 0;

    // Sorting
    
    /// <summary>
    /// Enable sorting for this column
    /// </summary>
    [Parameter] public bool Sortable { get; set; } = false;
    
    /// <summary>
    /// Name of the column which will be put into the <b>SortColumn</b> field of the request
    /// </summary>
    [Parameter] public string? SortingName { get; set; }

    public SortDirection? SortingDirection { get; set; } = null;

    protected override void OnInitialized()
    {
        Parent.AddColumn(this);
    }

    /// <summary>
    /// Define the content of the column header. If not overridden the default header will be rendered
    /// </summary>
    /// <param name="__builder"></param>
    protected internal virtual void RenderHeader(RenderTreeBuilder __builder)
    {
        if (HeaderTemplate != null)
            HeaderTemplate.Invoke(this).Invoke(__builder);
        else
            RenderDefaultHeader(__builder);
    }

    /// <summary>
    /// Define the cell content for the column per item. You should put your content in a <b>td</b> element to fit in the table properly
    /// </summary>
    /// <param name="__builder">Render builder from the parent grid</param>
    /// <param name="item">Item which should be used to determine the current cells content</param>
    protected internal abstract void RenderCell(RenderTreeBuilder __builder, TGridItem item);

    /// <summary>
    /// Callback whenever the items of the parent grid have changed. Needs to be overridden if you wish to handle it
    /// </summary>
    protected internal virtual ValueTask OnItemsChangedAsync()
        => ValueTask.CompletedTask;
}