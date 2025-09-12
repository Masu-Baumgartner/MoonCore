namespace MoonCore.Models;

public class CountedData<T> : ICountedData<T>
{
    /// <summary>
    /// TotalCount of the items inside the data source 
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Item excerpt from the data source
    /// </summary>
    public T[] Items { get; set; }
}