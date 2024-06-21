namespace MoonCore.Blazor.Models.Forms;

public class AutoCrudConfiguration<TItem>
{
    public Func<TItem, Task>? CustomAdd { get; set; }
    public Func<TItem, Task>? CustomUpdate { get; set; }
    public Func<TItem, Task>? CustomDelete { get; set; }
    
    public Func<TItem, Task>? ValidateAdd { get; set; }
    public Func<TItem, Task>? ValidateUpdate { get; set; }
    public Func<TItem, Task>? ValidateDelete { get; set; }
}