namespace MoonCore.Blazor.FlyonUi.Exceptions;

public class GlobalErrorService
{
    private GlobalErrorHandler? HandlerReference;
    private readonly IGlobalErrorFilter[] ErrorFilters;

    public GlobalErrorService(IEnumerable<IGlobalErrorFilter> errorFilters)
    {
        ErrorFilters = errorFilters.ToArray();
    }

    public async Task ShowMessage(string message)
        => await ShowMessage("Error", message);

    public async Task ShowMessage(string title, string message)
        => await ShowMessage(title, message, "icon-triangle-alert");

    public async Task ShowMessage(string title, string message, string icon)
    {
        if(HandlerReference == null)
            throw new AggregateException("HandlerReference is null. Make sure you have a GlobalErrorHandler component in your application");
    }

    public async Task HandleException(Exception ex)
        => await HandleException(ex, "An error occured:");

    public async Task HandleException(Exception ex, string title)
        => await HandleException(ex, title, false);

    public async Task HandleException(Exception ex, string title, bool isBlocking, bool isRecoverable = true)
    {
        // Filter exceptions which should not be handled by the global error service
        foreach (var errorFilter in ErrorFilters)
        {
            var isHandled = await errorFilter.HandleException(ex);
            
            if(isHandled)
                return;
        }
    }

    public void SetHandler(GlobalErrorHandler handler)
        => HandlerReference = handler;
}