using Microsoft.Extensions.Logging;

namespace MoonCore.Blazor.FlyonUi.Exceptions;

public class GlobalErrorService
{
    private GlobalErrorHandler? HandlerReference;
    private readonly IEnumerable<IGlobalErrorFilter> ErrorFilters;
    private readonly ILogger<GlobalErrorService> Logger;

    public GlobalErrorService(
        IEnumerable<IGlobalErrorFilter> errorFilters,
        ILogger<GlobalErrorService> logger
    )
    {
        Logger = logger;
        ErrorFilters = errorFilters;
    }

    public async Task ShowMessage(string message)
        => await ShowMessage("Error", message);

    public async Task ShowMessage(string title, string message)
        => await ShowMessage(title, message, "icon-triangle-alert");

    public async Task ShowMessage(string title, string message, string icon)
    {
        if (HandlerReference == null)
            throw new AggregateException(
                "HandlerReference is null. Make sure you have a GlobalErrorHandler component in your application");

        await HandlerReference.ShowMessage(title, message, icon);
    }

    public async Task HandleException(Exception ex)
        => await HandleException(ex, "An error occured:");

    public async Task HandleException(Exception ex, string title)
        => await HandleException(ex, title, false);

    public async Task HandleException(Exception ex, string title, bool isBlocking, bool isRecoverable = true)
    {
        if (HandlerReference == null)
            throw new AggregateException(
                "HandlerReference is null. Make sure you have a GlobalErrorHandler component in your application");

        // Filter exceptions which should not be handled by the global error service
        foreach (var errorFilter in ErrorFilters)
        {
            var isHandled = await errorFilter.HandleException(ex);

            if (isHandled)
                return;
        }

        await HandlerReference.HandleException(ex, title, isBlocking, isRecoverable);
        
        Logger.LogWarning(ex, "A exception is being handled by the global error handler");
    }

    public void SetHandler(GlobalErrorHandler handler)
        => HandlerReference = handler;
}