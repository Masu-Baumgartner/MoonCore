namespace MoonCore.Blazor.Models.Fast;

public class FastCrudConfiguration<TItem>
{
    public Func<TItem, Task>? CustomCreate { get; set; }
    public Func<TItem, Task>? CustomEdit { get; set; }
    public Func<TItem, Task>? CustomDelete { get; set; }

    public bool ExperimentalDisableItemCloning { get; set; } = false;
}