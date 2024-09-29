using MoonCore.Blazor.Tailwind.Toasts.Components;

namespace MoonCore.Blazor.Tailwind.Toasts;

public class ToastService
{
    private ToastLauncher ToastLauncher;

    public Task Success(string title, string text = "") => LaunchInternal<SuccessToast>(title, text);
    public Task Info(string title, string text = "") => LaunchInternal<InfoToast>(title, text);
    public Task Warning(string title, string text = "") => LaunchInternal<WarningToast>(title, text);
    public Task Danger(string title, string text = "") => LaunchInternal<DangerToast>(title, text);

    public async Task Progress(string title, string defaultText, Func<ProgressToast, Task> work)
    {
        await Launch<ProgressToast>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", defaultText);
            buildAttr.Add("Work", work);
        });
    }

    private async Task LaunchInternal<T>(string title, string text) where T : BaseToast
    {
        await Launch<T>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", text);
        }, (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
    }

    private Task<ToastItem> Launch<T>(Action<Dictionary<string, object>>? onConfigure = null, int hideDelay = -1)
        where T : BaseToast
        => ToastLauncher.Launch<T>(onConfigure, hideDelay);

    public void SetLauncher(ToastLauncher launcher) => ToastLauncher = launcher;
}