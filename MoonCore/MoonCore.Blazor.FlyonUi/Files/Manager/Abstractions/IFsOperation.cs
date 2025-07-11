namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFsOperation
{
    public string Name { get; }
    public string Icon { get; }
    public int Order { get; }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager);
}