namespace MoonCoreUI.Attributes;

public class RelatedItemsSelectorAttribute : Attribute
{
    public string SelectorProp { get; set; } = "";
    public string DisplayProp { get; set; } = "";
}