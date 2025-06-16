using MoonCore.Blazor.FlyonUi.Modals.Components;

namespace MoonCore.Blazor.FlyonUi.Modals;

public class ModalService
{
    private ModalLauncher ModalLauncher;
    
    public void SetLauncher(ModalLauncher launcher) => ModalLauncher = launcher;

    public Task<ModalItem> Launch<T>(Action<Dictionary<string, object>>? onConfigure = null, string size = "max-w-lg", bool allowUnfocusHide = false) where T : BaseModal
        => ModalLauncher.Launch<T>(onConfigure, size, allowUnfocusHide);
}