using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonCore.Common;

/// <summary>
/// Represents a pre-counted collection of items which may only contain
/// a smaller set (Excerpt) of the total items available at the data source
/// Can be used for providing access to remote paginated data using the DataGrid from Mooncore.Blazor.FlyonUI
/// </summary>
/// <typeparam name="T">Type of the items stored inside the instance</typeparam>
[JsonConverter(typeof(CountedDataConverterFactory))]
public class CountedData<T> : IEnumerable<T>
{
    /// <summary>
    /// Total count of items available in the source
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Excerpt of the items available in the source
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// Creates an empty collection of a pre-counted data source
    /// </summary>
    public CountedData()
    {
        Items = [];
        TotalCount = 0;
    }

    /// <summary>
    /// Creates an instance by using the provided items for the total count and the excerpt
    /// </summary>
    /// <param name="items">Items to create the instance from</param>
    public CountedData(IEnumerable<T> items)
    {
        var content = items.ToArray();
        
        Items = content;
        TotalCount = content.Length;
    }
    
    /// <summary>
    /// Creates an instance by using the provided items as the excerpt and the provided number as the total count
    /// </summary>
    /// <param name="items">Items to create the instance excerpt from</param>
    /// <param name="totalCount">Number to use as the total count</param>
    public CountedData(IEnumerable<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }

    /// <inhertitdoc />
    public IEnumerator<T> GetEnumerator()
        => Items.GetEnumerator();

    /// <inhertitdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

/// <summary>
/// Helper functions to quickly work with <see cref="CountedData{T}"/>
/// </summary>
public static class CountedData
{
    /// <summary>
    /// Queries all data using the provided callback
    /// </summary>
    /// <param name="callback">Callback to get the data from</param>
    /// <param name="count">Count of items to query per callback</param>
    /// <typeparam name="T">Type of the items to query</typeparam>
    /// <returns>Returns an array of all fetched items</returns>
    public static T[] All<T>(CountedLoaderSync<T> callback, int count = 50)
    {
        int totalCount;
        var startIndex = 0;
        var items = new List<T>();

        do
        {
            var data = callback.Invoke(startIndex, count);
            
            items.AddRange(data.Items);
            totalCount = data.TotalCount;

            startIndex += count;
        } while (startIndex < totalCount);
        
        return items.ToArray();
    }
    
    /// <summary>
    /// Queries all data using the provided callback
    /// </summary>
    /// <param name="callback">Callback to get the data from</param>
    /// <param name="count">Count of items to query per callback</param>
    /// <typeparam name="T">Type of the items to query</typeparam>
    /// <returns>Returns an array of all fetched items</returns>
    public static async Task<T[]> AllAsync<T>(CountedLoaderAsync<T> callback, int count = 50)
    {
        var startIndex = 0;
        var totalCount = 0;
        var items = new List<T>();

        do
        {
            var data = await callback.Invoke(startIndex, count);
            
            items.AddRange(data.Items);
            totalCount = data.TotalCount;

            startIndex += count;
        } while (startIndex < totalCount);
        
        return items.ToArray();
    }
    
    /// <summary>
    /// Queries all data using the provided callback
    /// </summary>
    /// <param name="callback">Callback to get the data from</param>
    /// <param name="count">Count of items to query per callback</param>
    /// <typeparam name="T">Type of the items to query</typeparam>
    /// <returns>Returns an array of all fetched items</returns>
    public static async ValueTask<T[]> AllAsync<T>(CountedLoaderValueAsync<T> callback, int count = 50)
    {
        var startIndex = 0;
        var totalCount = 0;
        var items = new List<T>();

        do
        {
            var data = await callback.Invoke(startIndex, count);
            
            items.AddRange(data.Items);
            totalCount = data.TotalCount;

            startIndex += count;
        } while (startIndex < totalCount);
        
        return items.ToArray();
    }
}

public delegate CountedData<T> CountedLoaderSync<T>(int startIndex, int count);
public delegate Task<CountedData<T>> CountedLoaderAsync<T>(int startIndex, int count);
public delegate ValueTask<CountedData<T>> CountedLoaderValueAsync<T>(int startIndex, int count);