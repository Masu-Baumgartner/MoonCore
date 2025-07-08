namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFileManager
{
    public long TransferChunkSize { get; }
    public Task Refresh();
    public Task CloseOpenScreen();
}