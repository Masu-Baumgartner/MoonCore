using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MoonCore.Blazor.FlyonUi.Grid.ToolbarItems;

public abstract partial class ToolbarItemBase<TGridItem>
{
    /// <summary>
    /// Reference to the DataGrid parent
    /// </summary>
    [CascadingParameter] public DataGrid<TGridItem> Parent { get; set; }
    
    /// <summary>
    /// Order number for the toolbar item. Higher values place it further to the right
    /// </summary>
    [Parameter] public int Order { get; set; }
    
    protected override void OnInitialized()
    {
        Parent.AddToolbarItem(this);
    }
    
    /// <summary>
    /// Render the content of the toolbar item in this function
    /// </summary>
    /// <param name="__builder">Render builder from the parent grid</param>
    public abstract void RenderContent(RenderTreeBuilder builder);
    
    /// <summary>
    /// Callback whenever the items of the parent grid have changed. Needs to be overridden if you wish to handle it
    /// </summary>
    protected internal virtual ValueTask OnItemsChangedAsync()
        => ValueTask.CompletedTask;
}