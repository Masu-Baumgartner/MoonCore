namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFileOperation
{
    public string Name { get; }
    public string Icon { get; }
    public string ContextCss { get; }
    public string ToolbarCss { get; }
    public int Order { get; }
    public bool OnlySingleFile { get; }
    public Func<FileEntry, bool>? Filter { get; }

    public Task Execute(string workingDir, FileEntry[] files, IFileAccess fileAccess, IFileManager fileManager);
}