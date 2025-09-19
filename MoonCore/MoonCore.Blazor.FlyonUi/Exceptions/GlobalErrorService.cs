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

    /// <summary>
    /// Shows the user an error message
    /// </summary>
    /// <param name="message">Message to show the user</param>
    public async Task ShowMessageAsync(string message)
        => await ShowMessageAsync("Error", message);

    /// <summary>
    /// Shows the user an error message inside a card with a title 
    /// </summary>
    /// <param name="title">Title of the error</param>
    /// <param name="message">Message of the error</param>
    public async Task ShowMessageAsync(string title, string message)
        => await ShowMessageAsync(title, message, "icon-triangle-alert");

    /// <summary>
    /// Shows the user an error message inside a card with a title and an icon
    /// </summary>
    /// <param name="title">Title of the error</param>
    /// <param name="message">Message of the error</param>
    /// <param name="icon">Icon to use for the error card. For a reference, look <see href="https://lucide.dev/icons">here</see></param>
    /// <exception cref="AggregateException">Throws when no GlobalErrorHandler component is defined within the view</exception>
    public async Task ShowMessageAsync(string title, string message, string icon)
    {
        if (HandlerReference == null)
            throw new AggregateException(
                "HandlerReference is null. Make sure you have a GlobalErrorHandler component in your application");

        await HandlerReference.ShowMessageAsync(title, message, icon);
    }

    /// <summary>
    /// Displays a user-friendly representation of the provided exception
    /// </summary>
    /// <param name="ex">Exception to display to the user</param>
    public async Task HandleExceptionAsync(Exception ex)
        => await HandleExceptionAsync(ex, "An error occured:");

    /// <summary>
    /// Displays a user-friendly representation of the provided exception
    /// </summary>
    /// <param name="ex">Exception to display to the user</param>
    /// <param name="title">Additional title to the display</param>
    public async Task HandleExceptionAsync(Exception ex, string title)
        => await HandleExceptionAsync(ex, title, false);

    /// <summary>
    /// Displays a user-friendly representation of the provided exception, specifying if it is blocking and recoverable
    /// </summary>
    /// <param name="ex">Exception to display to the user</param>
    /// <param name="title">Additional title to the display</param>
    /// <param name="isBlocking">Specifies if the exception should block the current user flow. If false the exception will be shown within a toast</param>
    /// <param name="isRecoverable">Specifies if the user can recover the application state ("Go Back" button) or needs to reload the page</param>
    public async Task HandleExceptionAsync(Exception ex, string title, bool isBlocking, bool isRecoverable = true)
    {
        if (HandlerReference == null)
            throw new AggregateException(
                "HandlerReference is null. Make sure you have a GlobalErrorHandler component in your application");

        // Filter exceptions which should not be handled by the global error service
        foreach (var errorFilter in ErrorFilters)
        {
            var isHandled = await errorFilter.HandleExceptionAsync(ex);

            if (isHandled)
                return;
        }

        await HandlerReference.HandleExceptionAsync(ex, title, isBlocking, isRecoverable);
        
        Logger.LogWarning(ex, "A exception is being handled by the global error handler");
    }

    internal void SetHandler(GlobalErrorHandler handler)
        => HandlerReference = handler;
}