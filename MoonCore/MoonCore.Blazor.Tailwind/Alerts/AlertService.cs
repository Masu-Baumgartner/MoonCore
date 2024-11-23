using MoonCore.Blazor.Tailwind.Alerts.Components;
using MoonCore.Blazor.Tailwind.Forms;
using MoonCore.Blazor.Tailwind.Modals.Components;
using MoonCore.Blazor.Tailwind.Modals;

namespace MoonCore.Blazor.Tailwind.Alerts;

public class AlertService
{
    private readonly ModalService ModalService;

    public AlertService(ModalService modalService)
    {
        ModalService = modalService;
    }

    public Task Success(string title, string text) => Launch<SuccessAlert>(title, text);
    public Task Info(string title, string text) => Launch<InfoAlert>(title, text);
    public Task Warning(string title, string text) => Launch<WarningAlert>(title, text);
    public Task Danger(string title, string text) => Launch<DangerAlert>(title, text);

    public async Task ConfirmDanger(string title, string text, Func<Task> confirmAction)
    {
        await ModalService.Launch<ConfirmDangerAlert>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", text);
            buildAttr.Add("ConfirmAction", confirmAction);
        });
    }

    private async Task Launch<T>(string title, string text) where T : BaseModal
    {
        await ModalService.Launch<T>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", text);
        });
    }

    public async Task FormAlert<T>(string title, Action<FormConfiguration<T>> onConfigure, Func<T, Task> onSubmit, T? initialModel = null)
        where T : class
    {
        await ModalService.Launch<FormAlert<T>>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("OnConfigureForm", onConfigure);
            buildAttr.Add("OnSubmit", onSubmit);
            buildAttr.Add("FormModel", initialModel);
        });
    }
}