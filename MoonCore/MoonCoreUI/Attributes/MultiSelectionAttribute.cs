namespace MoonCoreUI.Attributes;

public class MultiSelectionAttribute : Attribute
{
    public string DisplayProp { get; set; }
    public string SearchProp { get; set; }
    public string ItemColor { get; set; } = "secondary";
}