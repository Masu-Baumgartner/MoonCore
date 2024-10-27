using MoonCore.Blazor.Tailwind.Forms;
using MoonCore.Models;

namespace MoonCore.Blazor.Tailwind.Crud;

public class CrudOptions<TItem, TCreateForm, TUpdateForm>
{
    // Loader
    public Func<int, int, Task<IPagedData<TItem>>> ItemLoader { get; set; }
    public Func<string, Task<TItem?>> SingleItemLoader { get; set; }
    
    // Query state sync
    public Func<TItem, string>? QueryIdentifier { get; set; }
    
    // Form configurations
    public Action<FormConfiguration<TCreateForm>> OnConfigureCreate { get; set; }
    public Action<TItem, FormConfiguration<TUpdateForm>> OnConfigureUpdate { get; set; }
    
    // Data modification
    public Func<TCreateForm, Task> OnCreate { get; set; }
    public Func<TItem, TUpdateForm, Task> OnUpdate { get; set; }
    public Func<TItem, Task> OnDelete { get; set; }
    
    // Item Names
    public string ItemName { get; set; }
    public string ItemsName { get; set; }
    
    
    // Advanced configuration
    public string QueryStateId { get; set; } = "state";
    public string QueryItemId { get; set; } = "item";
    
    // Design
    public bool UseHeader { get; set; } = true;
    public bool UseBar { get; set; } = false;
    
    // View table
    public bool ShowViewTableBorder { get; set; } = false;
}