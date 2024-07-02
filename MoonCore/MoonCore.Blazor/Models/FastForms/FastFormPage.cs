using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.FastForms;

public class FastFormPage
{
    public string Name { get; set; }
    public RenderFragment? OverrideComponent { get; set; }
    public List<FastFormSection> Sections { get; set; } = new();
}