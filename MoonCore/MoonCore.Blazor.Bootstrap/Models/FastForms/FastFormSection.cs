using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms;

public class FastFormSection
{
    public string Name { get; set; }
    public string? Icon { get; set; }
    public List<ComponentBase> Components { get; set; } = new();
}