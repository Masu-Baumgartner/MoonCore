using MoonCore.Blazor.Bootstrap.Attributes.Auto;
using MoonCore.Blazor.Test.Data;

namespace MoonCore.Blazor.Test.Forms;

public class UpdateCarForm
{
    public string Name { get; set; }
    [Page("Plate stuff")]
    public string Plate { get; set; }

    [CustomComponent("OwnersSelect")] public List<User> Owners { get; set; } = new();
}