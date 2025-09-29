using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Modals;

public class ModalReference
{
    /// <summary>
    /// Content to render inside the modal 
    /// </summary>
    public RenderFragment Component { get; set; }
    
    /// <summary>
    /// Tailwind width class to define the size of the modal
    /// </summary>
    public string Size { get; set; }
    
    /// <summary>
    /// Toggles if clicking outside the modal (onto the backdrop) will hide the modal
    /// </summary>
    public bool AllowUnfocusHide { get; set; }
}