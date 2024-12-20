namespace MoonCore.Models;

public class PagedData<TItem> : IPagedData<TItem>
{
    public TItem[] Items { set; get; }
    public int CurrentPage { set; get; }
    public int TotalPages { set; get; }
    public int TotalItems { set; get; }
    public int PageSize { set; get; }

    // Do not use in production on large quantities of data
    public static PagedData<TItem> Create(IEnumerable<TItem> items, int page, int pageSize)
    {
        var count = items.Count();
        
        var finalItems = items
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArray();

        return new()
        {
            Items = finalItems,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = count,
            TotalPages = count == 0 ? 0 : count / pageSize
        };
    }

    public static async Task<TItem[]> All(Func<int, int, Task<PagedData<TItem>>> loader, int pageSize = 50)
    {
        var initialData = await loader.Invoke(0, pageSize);

        // Check if we already have all data
        if (initialData.Items.Length == initialData.TotalItems)
            return initialData.Items;
        
        var items = new List<TItem>();
        items.AddRange(initialData.Items);
        
        for (var page = 1; page < initialData.TotalPages; page++)
        {
            var data = await loader.Invoke(page, pageSize);
            items.AddRange(data.Items);
        }

        return items.ToArray();
    }
    
    public static TItem[] All(Func<int, int, PagedData<TItem>> loader, int pageSize = 50)
    {
        var initialData = loader.Invoke(0, pageSize);

        // Check if we already have all data
        if (initialData.Items.Length == initialData.TotalItems)
            return initialData.Items;
        
        var items = new List<TItem>();
        items.AddRange(initialData.Items);
        
        for (var page = 1; page < initialData.TotalPages; page++)
        {
            var data = loader.Invoke(page, pageSize);
            items.AddRange(data.Items);
        }

        return items.ToArray();
    }
}