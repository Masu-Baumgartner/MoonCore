namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IToolbarOperation : IFsOperation
{
    public string ToolbarCss { get; }

    public Task Execute(string workingDir, IFsAccess access, IFileManager fileManager);
}