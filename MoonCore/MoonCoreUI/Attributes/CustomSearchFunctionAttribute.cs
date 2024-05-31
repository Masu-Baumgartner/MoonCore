namespace MoonCoreUI.Attributes;

public class CustomSearchFunctionAttribute : Attribute
{
    public string Id { get; set; }
    
    public CustomSearchFunctionAttribute(string id)
    {
        Id = id;
    }
}