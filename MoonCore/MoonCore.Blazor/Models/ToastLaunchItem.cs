using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models;

public class ToastLaunchItem
{
    public int Id { get; set; }
    public TimeSpan Duration { get; set; }
    public RenderFragment Render { get; set; }
    public bool Initialized { get; set; } = false;
}