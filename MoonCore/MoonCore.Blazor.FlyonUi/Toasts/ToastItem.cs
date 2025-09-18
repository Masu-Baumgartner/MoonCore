using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Toasts;

public class ToastItem
{
    /// <summary>
    /// Content of the toast
    /// </summary>
    public RenderFragment Component { get; set; }
}