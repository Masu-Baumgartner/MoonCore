using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.Fast;

public class FastSection
{
    public string Name { get; set; }
    public string? Icon { get; set; }
    public List<RenderFragment> Renders { get; set; } = new();
}