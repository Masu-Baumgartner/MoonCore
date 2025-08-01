namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface ICombineAccess
{
    public Task Combine(string destination, string[] files);
}