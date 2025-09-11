namespace MoonCore.Blazor.FlyonUi.Grid;

public class DataGridItemResult<TGridItem>
{
    /// <summary>
    /// Items to process and show in the table
    /// </summary>
    public IEnumerable<TGridItem> Items { get; set; }
    
    /// <summary>
    /// Total count of retrievable items. Important for pagination
    /// </summary>
    public int TotalCount { get; set; }
}