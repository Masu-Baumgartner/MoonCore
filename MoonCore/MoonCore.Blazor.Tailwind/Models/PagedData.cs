namespace MoonCore.Blazor.Tailwind.Models;

public class PagedData<TItem> : IPagedData<TItem>
{
    public TItem[] Items { set; get; }
    public int CurrentPage { set; get; }
    public int TotalPages { set; get; }
    public int TotalItems { set; get; }
    public int PageSize { set; get; }

    public static PagedData<TItem> Create(TItem[] items, int page, int pageSize)
    {
        var count = items.Length;
        
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