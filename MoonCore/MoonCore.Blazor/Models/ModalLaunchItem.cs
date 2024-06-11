using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models;

public class ModalLaunchItem
{
    public int Id { get; set; }
    public string CssClasses { get; set; }
    public RenderFragment Render { get; set; }
    public bool Focus { get; set; }
    public bool Initialized { get; set; } = false;
}