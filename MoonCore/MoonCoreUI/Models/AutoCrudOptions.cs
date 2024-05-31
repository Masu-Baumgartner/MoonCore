using MoonCore.Abstractions;

namespace MoonCoreUI.Models;

public class AutoCrudOptions
{
    public readonly Dictionary<string, object> CustomItemLoaders = new();
    public readonly Dictionary<string, object> CustomSearchFunctions = new();
    public readonly Dictionary<string, object> CustomDisplayFunctions = new();
    public readonly Dictionary<string, Type> CustomFormComponents = new();

    public string CreateText { get; set; }
    public string ItemName { get; set; } = "";

    public bool ShowSelect { get; set; } = true;

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

    public void AddCustomItemLoader<TItem, T>(string id, Func<Repository<T>, TItem?, IEnumerable<T>> function) where T : class
    {
        CustomItemLoaders[id] = function;
    }

    public bool TryGetCastedOption<T>(Func<AutoCrudOptions, Dictionary<string, object>> selector, string id, out T result)
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