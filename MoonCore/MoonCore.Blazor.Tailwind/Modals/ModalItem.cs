using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.Modals;

public class ModalItem
{
    public RenderFragment Component { get; set; }
    public string Size { get; set; }
}