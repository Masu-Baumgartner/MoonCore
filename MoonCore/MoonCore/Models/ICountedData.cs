namespace MoonCore.Models;

public interface ICountedData<out T>
{
    /// <summary>
    /// TotalCount of the items inside the data source 
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// Item excerpt from the data source
    /// </summary>
    public T[] Items { get; }
}