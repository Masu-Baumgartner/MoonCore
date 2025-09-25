using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Drawers;

/// <summary>
/// Reference to an active drawer
/// </summary>
public class DrawerReference
{
    /// <summary>
    /// Whether a click on the backdrop should close the drawer
    /// </summary>
    public bool AllowUnfocusHide { get; set; }
    
    /// <summary>
    /// Toggles if the drawer is visible. Used for the animation
    /// </summary>
    public bool IsVisible { get; set; }
    
    /// <summary>
    /// Content of the drawer
    /// </summary>
    public RenderFragment RenderFragment { get; set; }
    
    /// <summary>
    /// Direction in which the drawer should be opened
    /// </summary>
    public DrawerDirection Direction { get; set; }
}