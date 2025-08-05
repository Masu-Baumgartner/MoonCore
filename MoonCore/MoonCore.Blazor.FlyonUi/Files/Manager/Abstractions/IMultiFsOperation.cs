namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IMultiFsOperation : IFsOperation
{
    public string ContextCss { get; }
    public string ToolbarCss { get; }
    public FilePermissions RequiredPermissions { get; }

    public Task Execute(string workingDir, FsEntry[] entries, IFsAccess access, IFileManager fileManager);
}