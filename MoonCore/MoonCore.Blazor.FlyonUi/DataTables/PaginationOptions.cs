namespace MoonCore.Blazor.FlyonUi.DataTables;

public class PaginationOptions
{
    /// <summary>
    /// Page of the items
    /// </summary>
    public int Page { get; set; } = 0;
    
    /// <summary>
    /// Amount of items
    /// </summary>
    public int PerPage { get; set; } = 20;
}