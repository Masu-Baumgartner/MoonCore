namespace MoonCore.Blazor.Models.FastForms;

public class FastCrudConfiguration<TItem>
{
    public Func<TItem, Task>? CustomCreate { get; set; }
    public Func<TItem, Task>? CustomEdit { get; set; }
    public Func<TItem, Task>? CustomDelete { get; set; }
    
    public Func<TItem, Task>? ValidateCreate { get; set; }
    public Func<TItem, Task>? ValidateEdit { get; set; }
    public Func<TItem, Task>? ValidateDelete { get; set; }

    public bool ExperimentalDisableItemCloning { get; set; } = false;
}