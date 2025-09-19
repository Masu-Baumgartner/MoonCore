namespace MoonCore.Blazor.FlyonUi.Exceptions;

public interface IGlobalErrorFilter
{
    public Task<bool> HandleExceptionAsync(Exception ex);
}