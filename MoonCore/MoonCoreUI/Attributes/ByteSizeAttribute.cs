namespace MoonCoreUI.Attributes;

public class ByteSizeAttribute : Attribute
{
    public int Converter { get; set; } = 0;
    public int MinimumUnit { get; set; } = 0;
    public int DefaultUnit { get; set; } = 0;
}