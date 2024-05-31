namespace MoonCoreUI.Attributes;

public class CustomFormComponentAttribute : Attribute
{
    public string Id { get; set; }
    
    public CustomFormComponentAttribute(string id)
    {
        Id = id;
    }
}