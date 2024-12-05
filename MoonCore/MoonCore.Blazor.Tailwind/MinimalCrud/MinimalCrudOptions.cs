using MoonCore.Models;

namespace MoonCore.Blazor.Tailwind.MinimalCrud;

public class MinimalCrudOptions<T>
{
    public string Title { get; set; } = "";
    public Func<int, int, Task<IPagedData<T>>> ItemLoader { get; set; }
    public int PageSize { get; set; } = 15;

    public string? CreateUrl { get; set; }

    public Func<T, Task>? DeleteFunction { get; set; }
    public Func<T, string>? UpdateUrl { get; set; }
    public Func<T, string>? DeleteUrl { get; set; }
}