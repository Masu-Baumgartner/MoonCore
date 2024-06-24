namespace MoonCore.Blazor.Test.Data;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Plate { get; set; } = "";
    public bool IsElectric { get; set; } = false;
    public CarDriverType DriverType { get; set; }
}