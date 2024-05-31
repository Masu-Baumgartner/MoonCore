namespace MoonCoreUI.Attributes;

public class CustomDisplayFunctionAttribute : Attribute
{
    public string Id { get; set; }
    
    public CustomDisplayFunctionAttribute(string id)
    {
        Id = id;
    }
}