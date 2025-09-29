using MoonCore.Blazor.FlyonUi.Toasts.Components;

namespace MoonCore.Blazor.FlyonUi.Toasts;

public class ToastService
{
    private ToastLauncher ToastLauncher;

    /// <summary>
    /// Launches a success toast with the provided text
    /// </summary>
    /// <param name="title">Title of the toast</param>
    /// <param name="text"><b>Optional:</b> Content of the toast</param>
    /// <returns></returns>
    public Task SuccessAsync(string title, string text = "") => LaunchInternalAsync<SuccessToast>(title, text);
    
    /// <summary>
    /// Launches an info toast with the provided text
    /// </summary>
    /// <param name="title">Title of the toast</param>
    /// <param name="text"><b>Optional:</b> Content of the toast</param>
    /// <returns></returns>
    public Task InfoAsync(string title, string text = "") => LaunchInternalAsync<InfoToast>(title, text);
    
    /// <summary>
    /// Launches a warning toast with the provided text
    /// </summary>
    /// <param name="title">Title of the toast</param>
    /// <param name="text"><b>Optional:</b> Content of the toast</param>
    /// <returns></returns>
    public Task WarningAsync(string title, string text = "") => LaunchInternalAsync<WarningToast>(title, text);
    
    /// <summary>
    /// Launches an error toast with the provided text
    /// </summary>
    /// <param name="title">Title of the toast</param>
    /// <param name="text"><b>Optional:</b> Content of the toast</param>
    /// <returns></returns>
    public Task ErrorAsync(string title, string text = "") => LaunchInternalAsync<ErrorToast>(title, text);

    public async Task ProgressAsync(string title, string defaultText, Func<ProgressToast, Task> work)
    {
        await LaunchAsync<ProgressToast>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", defaultText);
            buildAttr.Add("Work", work);
        }, -1);
    }

    private async Task LaunchInternalAsync<T>(string title, string text) where T : BaseToast
    {
        await LaunchAsync<T>(buildAttr =>
        {
            buildAttr.Add("Title", title);
            buildAttr.Add("Text", text);
        });
    }

    /// <summary>
    /// Launches a component inside a toast container and automatically hides it again
    /// </summary>
    /// <param name="onConfigure">Callback to configure the parameters of the component</param>
    /// <param name="hideDelayMs">Time in milliseconds until the modal should hide again. Use <b>-1</b> to disable this. Default: <b>5s</b></param>
    /// <typeparam name="T">Type of the component</typeparam>
    /// <returns>Reference item to close the toast using <see cref="CloseAsync"/></returns>
    public Task<ToastReference> LaunchAsync<T>(Action<Dictionary<string, object>>? onConfigure = null, int hideDelayMs = 5000)
        where T : BaseToast
        => ToastLauncher.LaunchAsync<T>(onConfigure, hideDelayMs);

    /// <summary>
    /// Closes the provided toast
    /// </summary>
    /// <param name="reference">Reference item of the toast to close</param>
    public Task CloseAsync(ToastReference reference)
        => ToastLauncher.CloseAsync(reference);

    internal void SetLauncher(ToastLauncher launcher) => ToastLauncher = launcher;
}