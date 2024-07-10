using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.Models;

public class ModalLaunchItem
{
    public string Id { get; set; }
    public RenderFragment Render { get; set; }
}