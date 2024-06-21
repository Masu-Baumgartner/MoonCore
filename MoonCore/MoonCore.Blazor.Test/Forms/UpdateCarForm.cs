using MoonCore.Blazor.Attributes.Auto;

namespace MoonCore.Blazor.Test.Forms;

public class UpdateCarForm
{
    public string Name { get; set; }
    [Page("Plate stuff")]
    public string Plate { get; set; }
}