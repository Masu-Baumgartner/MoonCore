namespace MoonCore.Blazor.FlyonUi.Common;

/// <summary>
/// Represents the options in which a specific column should be sorted
/// </summary>
public record SortOption
{
    /// <summary>
    /// Name of the column to apply the sorting
    /// </summary>
    public string Column { get; }
    
    /// <summary>
    /// Direction to sort the column in
    /// </summary>
    public SortDirection Direction { get; }
    
    /// <summary>
    /// Creates an instance of the sort option specifying name and direction
    /// </summary>
    /// <param name="column">Name of the column to apply the sorting</param>
    /// <param name="direction">Direction to sort the column in</param>
    public SortOption(string column, SortDirection direction)
    {
        Column = column;
        Direction = direction;
    }
}