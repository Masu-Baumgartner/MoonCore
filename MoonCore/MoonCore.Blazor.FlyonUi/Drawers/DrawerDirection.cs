namespace MoonCore.Blazor.FlyonUi.Drawers;

/// <summary>
/// Direction where the 
/// </summary>
public enum DrawerDirection
{
    /// <summary>
    /// Aligns the drawer to the top of the viewport.
    /// Will result in the following css classes being used
    /// <c>overlay-open:translate-y-0 drawer-top</c>
    /// </summary>
    Top,
    /// <summary>
    /// Aligns the drawer to the bottom of the viewport.
    /// Will result in the following css classes being used
    /// <c>overlay-open:translate-y-0 drawer-bottom</c>
    /// </summary>
    Bottom,
    /// <summary>
    /// Aligns the drawer to the left of the viewport.
    /// Will result in the following css classes being used
    /// <c>overlay-open:translate-x-0 drawer-start</c>
    /// </summary>
    Left,
    /// <summary>
    /// Aligns the drawer to the right of the viewport.
    /// Will result in the following css classes being used
    /// <c>overlay-open:translate-x-0 drawer-end</c>
    /// </summary>
    Right
}