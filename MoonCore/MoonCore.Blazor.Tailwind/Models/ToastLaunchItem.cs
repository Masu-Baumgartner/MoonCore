using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.Models;

public class ToastLaunchItem
{
    public string Id { get; set; }
    public RenderFragment Render { get; set; }
}