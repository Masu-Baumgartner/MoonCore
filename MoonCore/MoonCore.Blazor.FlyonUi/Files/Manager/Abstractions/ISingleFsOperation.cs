namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface ISingleFsOperation : IFsOperation
{
    public string ContextCss { get; }
    public Func<FsEntry, bool>? Filter { get; }

    public Task Execute(string workingDir, FsEntry entry, IFsAccess access, IFileManager fileManager);
}