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
    
    public delegate ValueTask<CountedData<TItem>> CountedDataLoader<TItem>(int startIndex, int count);

    public static async ValueTask<TItem[]> LoadAllAsync<TItem>(CountedDataLoader<TItem> loader, int count = 50)
    {
        var startIndex = 0;
        var totalCount = 0;
        var items = new List<TItem>();

        do
        {
            var data = await loader(startIndex, count);
            
            items.AddRange(data.Items);
            totalCount = data.TotalCount;

            startIndex += count;
        } while (startIndex < totalCount);
        
        return items.ToArray();
    }
}