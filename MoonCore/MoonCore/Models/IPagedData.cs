namespace MoonCore.Models;

/// <summary>
/// Representation of paged data retrieved in an operation
/// </summary>
/// <typeparam name="TItem">Type of the items retrieved</typeparam>
public interface IPagedData<TItem>
{
    /// <summary>
    /// Retrieved items in operation
    /// </summary>
    public TItem[] Items { get; }
    
    /// <summary>
    /// Current requested page
    /// </summary>
    public int CurrentPage { get; }
    
    /// <summary>
    /// Total available pages
    /// </summary>
    public int TotalPages { get; }
    
    /// <summary>
    /// Amount of total available items
    /// </summary>
    public int TotalItems { get; }
    
    /// <summary>
    /// Page size used in the operation
    /// </summary>
    public int PageSize { get; }
}