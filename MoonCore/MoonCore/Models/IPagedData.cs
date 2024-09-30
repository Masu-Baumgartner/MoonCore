namespace MoonCore.Models;

public interface IPagedData<TItem>
{
    public TItem[] Items { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int TotalItems { get; }
    public int PageSize { get; }
}