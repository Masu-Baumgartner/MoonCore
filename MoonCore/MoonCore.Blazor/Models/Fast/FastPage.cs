using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.Fast;

public class FastPage
{
    public string Name { get; set; }
    public List<FastSection> Sections { get; set; } = new();
    public RenderFragment? OverrideComponent { get; set; }
}