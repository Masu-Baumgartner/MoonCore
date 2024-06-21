namespace MoonCoreUI.Components.NewAutoCrud;

public class AutoCrudContext2<TItem>
{
    public TItem? CurrentItem { get; set; }
    public AutoCrudState State { get; set; }
}