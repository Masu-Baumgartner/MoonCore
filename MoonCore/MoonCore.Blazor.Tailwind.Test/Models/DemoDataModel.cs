namespace MoonCore.Blazor.Tailwind.Test.Models;

public class DemoDataModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Flag { get; set; }
    public string Email { get; set; }

    public DemoDataModel Model { get; set; }
    public int RelationalDataId { get; set; }
    public SomeCoolEnum CoolEnum { get; set; }
}