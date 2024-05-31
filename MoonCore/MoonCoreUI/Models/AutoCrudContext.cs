namespace MoonCoreUI.Models;

public class AutoCrudContext
{
    public object? ItemToEdit { get; set; }
    public object CreateForm { get; set; }
    public object EditForm { get; set; }

    public T GetItemToEdit<T>() => (T)ItemToEdit!;
}