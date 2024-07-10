using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Bootstrap.Models.Forms;

public class FormSection
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public List<RenderFragment> Components { get; set; } = new();
}