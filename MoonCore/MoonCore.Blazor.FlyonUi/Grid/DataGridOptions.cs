using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Grid;

/// <summary>
/// Options for the data grid
/// </summary>
/// <typeparam name="TGridItem">Type the data grid displays</typeparam>
public class DataGridOptions<TGridItem>
{
    /// <summary>
    /// Options for the navigation of the data grid
    /// </summary>
    public NavigationOptions Navigation { get; } = new();
    
    /// <summary>
    /// Filtering options for the data grid
    /// </summary>
    public FilteringOptions Filtering { get; } = new();
    
    /// <summary>
    /// Sorting options for the data grid
    /// </summary>
    public SortingOptions Sorting { get; } = new();
    
    /// <summary>
    /// Customization options for the data grid
    /// </summary>
    public CustomizationOptions Customization { get; } = new();

    public class NavigationOptions
    {
        /// <summary>
        /// StartIndex to search in the data source
        /// </summary>
        public int StartIndex { get; set; } = 0;

        /// <summary>
        /// Amount of items to retrieve from the data source
        /// </summary>
        public int Count { get; set; } = 15;

        /// <summary>
        /// Toggle to enable the pagination component on the table
        /// </summary>
        public bool EnablePagination { get; set; }
        
        /// <summary>
        /// Page sizes to offer the user to select from
        /// </summary>
        public int[] Pages { get; set; } = [15, 30, 100];
    }
    
    public class FilteringOptions
    {
        /// <summary>
        /// Toggle to enable the filtering / searching component
        /// </summary>
        public bool Enable { get; set; }
        
        /// <summary>
        /// Toggle to enable the searching/filtering of the data source in realtime to the user input
        /// </summary>
        public bool EnableRealtime { get; set; }
    }
    
    public class SortingOptions
    {
        /// <summary>
        /// Enable the sorting capabilities of the data grid
        /// </summary>
        public bool Enable { get; set; }
    }
    
    public class CustomizationOptions
    {
        /// <summary>
        /// Used to customize the <i>tr</i> tag where the columns are being rendered within
        /// </summary>
        public Func<RenderFragment, TGridItem, RenderFragment>? ColumnsContainer { get; set; }
        
        /// <summary>
        /// Customize the content shown when no items are found
        /// </summary>
        public RenderFragment? NoItemsContent { get; set; }
    }
}