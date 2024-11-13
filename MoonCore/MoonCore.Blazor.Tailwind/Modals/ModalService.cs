using MoonCore.Blazor.Tailwind.Modals.Components;

namespace MoonCore.Blazor.Tailwind.Modals;

public class ModalService
{
    private ModalLauncher ModalLauncher;
    
    public void SetLauncher(ModalLauncher launcher) => ModalLauncher = launcher;

    public Task<ModalItem> Launch<T>(Action<Dictionary<string, object>>? onConfigure = null, string size = "max-w-lg") where T : BaseModal
        => ModalLauncher.Launch<T>(onConfigure, size);
}