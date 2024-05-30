namespace MoonCoreUI.Attributes;

public class MultiSelectionAttribute : Attribute
{
    public string DisplayField { get; set; } = "";
    public string SearchProp { get; set; } = "";
    public string Icon { get; set; } = "";

    public MultiSelectionAttribute(string displayField, string searchProp)
    {
        DisplayField = displayField;
        SearchProp = searchProp;
    }
}