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
}