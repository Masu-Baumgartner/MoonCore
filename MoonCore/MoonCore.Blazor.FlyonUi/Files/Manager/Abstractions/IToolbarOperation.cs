namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IToolbarOperation
{
    public string Name { get; }
    public string Icon { get; }
    public string Css { get; }
    public int Order { get; }
    
    public Task Execute(string workingDir, IFileAccess fileAccess, IFileManager fileManager);
}