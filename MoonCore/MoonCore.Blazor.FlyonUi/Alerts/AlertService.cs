using MoonCore.Blazor.FlyonUi.Alerts.Components;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Modals.Components;

namespace MoonCore.Blazor.FlyonUi.Alerts;

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
    public Task Error(string title, string text) => Launch<ErrorAlert>(title, text);

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
        }, allowUnfocusHide: true);
    }
}