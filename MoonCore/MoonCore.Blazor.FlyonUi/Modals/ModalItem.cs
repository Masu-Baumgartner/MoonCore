using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Modals;

public class ModalItem
{
    public RenderFragment Component { get; set; }
    public string Size { get; set; }
    public bool AllowUnfocusHide { get; set; }
}