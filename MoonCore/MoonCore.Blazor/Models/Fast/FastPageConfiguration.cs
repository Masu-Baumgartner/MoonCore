using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.Fast;

public class FastPageConfiguration
{
    public string Name { get; set; }
    public RenderFragment? OverrideComponent { get; set; }
}