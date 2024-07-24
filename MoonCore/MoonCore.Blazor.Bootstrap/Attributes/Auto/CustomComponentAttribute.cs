namespace MoonCore.Blazor.Bootstrap.Attributes.Auto;

public class CustomComponentAttribute : Attribute
{
    public string Name { get; set; }
    
    public CustomComponentAttribute(string name)
    {
        Name = name;
    }
}