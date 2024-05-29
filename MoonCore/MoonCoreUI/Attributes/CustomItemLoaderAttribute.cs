namespace MoonCoreUI.Attributes;

public class CustomItemLoaderAttribute : Attribute
{
    public string Id { get; set; }

    public CustomItemLoaderAttribute(string id)
    {
        Id = id;
    }
}