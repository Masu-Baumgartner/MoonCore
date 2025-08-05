using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class ArchiveOperation : IMultiFsOperation
{
    public string Name => "Archive";
    public string Icon => "icon-folder-archive";
    public string ContextCss => "text-success";
    public string ToolbarCss => "btn-success";
    public FilePermissions RequiredPermissions => FilePermissions.ReadWrite;
    public int Order => 0;

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => access is IArchiveAccess;

    public async Task Execute(string workingDir, FsEntry[] files, IFsAccess fsAccess, IFileManager fileManager)
    {
        
    }
}