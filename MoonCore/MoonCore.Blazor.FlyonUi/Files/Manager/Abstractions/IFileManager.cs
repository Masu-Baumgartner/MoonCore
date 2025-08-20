namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFileManager
{
    public Task Refresh();
    public Task CloseOpenScreen();
}