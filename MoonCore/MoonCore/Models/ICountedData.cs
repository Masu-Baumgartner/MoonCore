namespace MoonCore.Models;

/// <summary>
/// Representation of an excerpt of items from a data source
/// </summary>
/// <typeparam name="T">Type of the items</typeparam>
public interface ICountedData<out T>
{
    /// <summary>
    /// TotalCount of the items inside the data source 
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// Items excerpt from the data source
    /// </summary>
    public T[] Items { get; }
}