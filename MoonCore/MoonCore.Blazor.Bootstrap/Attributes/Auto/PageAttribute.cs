namespace MoonCore.Blazor.Bootstrap.Attributes.Auto;

public class PageAttribute : Attribute
{
    public string Name { get; set; }
    
    public PageAttribute(string name)
    {
        Name = name;
    }
}