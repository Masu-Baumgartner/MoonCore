namespace MoonCore.Blazor.FlyonUi.Exceptions;

public interface IGlobalErrorFilter
{
    public Task<bool> HandleException(Exception ex);
}