using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.FastForms;

public class FastFormSection
{
    public string Name { get; set; }
    public string? Icon { get; set; }
    public List<FastFormComponent> Components { get; set; } = new();
}