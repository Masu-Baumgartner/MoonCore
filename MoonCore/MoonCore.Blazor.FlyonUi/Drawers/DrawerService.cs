namespace MoonCore.Blazor.FlyonUi.Drawers;

/// <summary>
/// Provides access to the current drawer launcher for spawning and closing drawers
/// </summary>
public class DrawerService
{
    private DrawerLauncher Launcher;

    /// <summary>
    /// Launches a new drawer instance of the specified type
    /// </summary>
    /// <param name="parameters">Callback for providing parameters to the drawer component</param>
    /// <param name="unfocusHide">Toggles if clicking on the backdrop will close the drawer</param>
    /// <param name="direction">Sets the direction in which the drawer should be opened</param>
    /// <typeparam name="T">Type of the component to launch</typeparam>
    /// <returns>Reference to the drawer instance which can be used for <see cref="CloseAsync"/></returns>
    public async Task<DrawerReference> LaunchAsync<T>(
        Action<Dictionary<string, object>>? parameters = null,
        bool unfocusHide = false,
        DrawerDirection direction = DrawerDirection.Left
    ) where T : DrawerBase
    {
        return await Launcher.LaunchAsync<T>(parameters, unfocusHide, direction);
    }

    /// <summary>
    /// Closes the provided drawer reference
    /// </summary>
    /// <param name="reference">Reference of the drawer</param>
    public async Task CloseAsync(DrawerReference reference)
        => await Launcher.CloseAsync(reference);

    internal void SetLauncher(DrawerLauncher launcher)
        => Launcher = launcher;
}