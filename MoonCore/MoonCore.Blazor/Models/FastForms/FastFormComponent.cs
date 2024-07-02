using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.FastForms;

public class FastFormComponent
{
    public IFastFormPropertyConfiguration Configuration { get; set; }
    public DynamicComponent DynamicComponent { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}