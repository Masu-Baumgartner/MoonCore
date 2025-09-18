using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MoonCore.Blazor.FlyonUi.Grid.Rows;

public abstract partial class RowBase<TGridItem>
{
    /// <summary>
    /// Reference to the DataGrid parent
    /// </summary>
    [CascadingParameter] public DataGrid<TGridItem> Parent { get; set; }
    
    /// <summary>
    /// Order number for the row. Higher values place it further to the right
    /// </summary>
    [Parameter] public int Order { get; set; }
    
    protected override void OnInitialized()
    {
        Parent.AddRow(this);
    }
    
    /// <summary>
    /// Render the content of the row in this function. You should include a <b>tr</b> to have it properly formatted
    /// </summary>
    /// <param name="__builder">Render builder from the parent grid</param>
    public abstract void RenderContent(RenderTreeBuilder builder);
    
    /// <summary>
    /// Callback whenever the items of the parent grid have changed. Needs to be overridden if you wish to handle it
    /// </summary>
    protected internal virtual ValueTask OnItemsChangedAsync()
        => ValueTask.CompletedTask;
}