namespace MoonCore.Blazor.FlyonUi.Modals;

public class ModalService
{
    private ModalLauncher ModalLauncher;
    
    public void SetLauncher(ModalLauncher launcher) => ModalLauncher = launcher;

    /// <summary>
    /// Launch the provided component inside a modal container
    /// </summary>
    /// <param name="onConfigure"><b>Optional:</b> Callback to configure the components parameters</param>
    /// <param name="size"><b>Optional:</b> Tailwind width class to define the modal size</param>
    /// <param name="allowUnfocusHide"><b>Optional:</b>  Toggles if clicking outside the modal (onto the backdrop) will hide the modal</param>
    /// <typeparam name="T">Type of the component</typeparam>
    /// <returns>ModalItem to close the modal using <see cref="CloseAsync"/></returns>
    public Task<ModalReference> LaunchAsync<T>(Action<Dictionary<string, object>>? onConfigure = null, string size = "max-w-lg", bool allowUnfocusHide = false) where T : BaseModal
        => ModalLauncher.LaunchAsync<T>(onConfigure, size, allowUnfocusHide);

    /// <summary>
    /// Closes the provided modal
    /// </summary>
    /// <param name="reference">Reference item to the active modal</param>
    public Task CloseAsync(ModalReference reference)
        => ModalLauncher.CloseAsync(reference);
}