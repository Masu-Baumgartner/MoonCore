using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Toasts;

public class ToastReference
{
    /// <summary>
    /// Content of the toast
    /// </summary>
    public RenderFragment Component { get; set; }
}