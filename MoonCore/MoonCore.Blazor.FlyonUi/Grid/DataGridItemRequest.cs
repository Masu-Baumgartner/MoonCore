namespace MoonCore.Blazor.FlyonUi.Grid;

public class DataGridItemRequest
{
    /// <summary>
    /// Start index for retrieving the items. Starts at 0
    /// </summary>
    public int StartIndex { get; set; }
    
    /// <summary>
    /// Amount of items to retrieve
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Filter to search the items
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// Column to apply the sort direction to
    /// </summary>
    public string? SortColumn { get; set; }
    
    /// <summary>
    /// Direction to sort the items by the provided <see cref="SortColumn"/> with
    /// </summary>
    public SortState SortDirection { get; set; }
}