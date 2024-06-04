using MoonCore.Abstractions;

namespace MoonCoreUI.Components.NewAutoCrud;

public class AutoCrudOptions2<TItem>
{
    public readonly Dictionary<string, object> CustomItemLoaders = new();
    public readonly Dictionary<string, object> CustomSearchFunctions = new();
    public readonly Dictionary<string, object> CustomDisplayFunctions = new();
    public readonly Dictionary<string, Type> CustomFormComponents = new();
    
    public Func<TItem, Task>? ValidateEdit { get; set; }
    public Func<TItem, Task>? ValidateCreate { get; set; }
    public Func<TItem, Task>? ValidateDelete { get; set; }
    
    public Func<TItem, Task>? CustomEdit { get; set; }
    public Func<TItem, Task>? CustomCreate { get; set; }
    public Func<TItem, Task>? CustomDelete { get; set; }

    public void AddCustomFormComponent<T>(string id)
    {
        CustomFormComponents[id] = typeof(T);
    }

    public void AddCustomSearchFunction<T>(string id, Func<T, string> searchFunction)
    {
        CustomSearchFunctions[id] = searchFunction;
    }
    
    public void AddCustomDisplayFunction<T>(string id, Func<T, string> displayFunction)
    {
        CustomDisplayFunctions[id] = displayFunction;
    }

    public void AddCustomItemLoader<T>(string id, Func<Repository<T>, TItem?, IEnumerable<T>> function) where T : class
    {
        CustomItemLoaders[id] = function;
    }

    public bool TryGetCastedOption<T>(Func<AutoCrudOptions2<TItem>, Dictionary<string, object>> selector, string id, out T result)
    {
        var obj = selector.Invoke(this);

        if (!obj.TryGetValue(id, out var val))
        {
            result = default!;
            return false;
        }

        result = (T)val;
        return true;
    }
}