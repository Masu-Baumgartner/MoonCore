namespace MoonCore.Blazor.Bootstrap.Attributes.Auto;

public class ByteSizeInputAttribute : Attribute
{
    public string MinimumUnit { get; set; } = "KB";
    public string DefaultUnit { get; set; } = "KB";
    public int Converter { get; set; } = 1;
}