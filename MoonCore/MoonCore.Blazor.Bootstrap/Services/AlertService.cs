using MoonCore.Blazor.Bootstrap.Components.Alerts;

namespace MoonCore.Blazor.Bootstrap.Services;

public class AlertService
{
    private readonly ModalService ModalService;

    public AlertService(ModalService modalService)
    {
        ModalService = modalService;
    }

    private async Task NotificationModal(string color, string text, string icon)
    {
        await ModalService.Launch<ModalNotificationAlert>(cssClasses: "modal-dialog-centered", buildAttributes:
            attributes =>
            {
                attributes.Add("Content", text);
                attributes.Add("Color", color);
                attributes.Add("Icon", icon);
            }
        );
    }

    public async Task Success(string text) => await NotificationModal("success", text, "bx-check");
    
    public async Task Info(string text) => await NotificationModal("info", text, "bx-info-circle");
    
    public async Task Warning(string text) => await NotificationModal("warning", text, "bx-error");
    
    public async Task Danger(string text) => await NotificationModal("danger", text, "bx-error-alt");

    public async Task Confirm(string title, string text, Func<Task> func, string yes = "Confirm", string no = "Cancel")
    {
        await ModalService.Launch<ModalConfirmAlert>(cssClasses: "modal-dialog-centered", buildAttributes:
            attributes =>
            {
                attributes.Add("Title", title);
                attributes.Add("Text", text);
                attributes.Add("Func", func);
                attributes.Add("Yes", yes);
                attributes.Add("No", no);
            }
        );
    }
    
    public async Task Text(string title, string text, Func<string, Task> func, string placeholder = "", string yes = "Confirm", string no = "Cancel")
    {
        await ModalService.Launch<ModalTextAlert>(cssClasses: "modal-dialog-centered", buildAttributes:
            attributes =>
            {
                attributes.Add("Title", title);
                attributes.Add("Text", text);
                attributes.Add("Func", func);
                attributes.Add("Yes", yes);
                attributes.Add("No", no);
                attributes.Add("Placeholder", placeholder);
            }
        );
    }
}