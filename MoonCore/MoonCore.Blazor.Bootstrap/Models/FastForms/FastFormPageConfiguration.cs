using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms;

public class FastFormPageConfiguration
{
    public string Name { get; set; }
    public RenderFragment? OverrideComponent { get; set; }
}