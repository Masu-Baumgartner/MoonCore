using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
    [Parameter] public string? SortName { get; set; }

    public SortState SortState { get; set; } = SortState.None;

    protected override void OnInitialized()
    {
        Parent.AddColumn(this);
    }

    // 
    protected internal virtual void RenderHeader(RenderTreeBuilder __builder)
    {
        if (HeaderTemplate != null)
            HeaderTemplate.Invoke(this).Invoke(__builder);
        else
            RenderDefaultHeader(__builder);
    }

    protected internal abstract void RenderCell(RenderTreeBuilder __builder, TGridItem item);

    protected internal virtual ValueTask OnItemsChangedAsync()
        => ValueTask.CompletedTask;
}