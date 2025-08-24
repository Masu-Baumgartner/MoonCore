using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFsOpenOperation
{
    public Func<FsEntry, bool> Filter { get; }
    public int Order { get; }
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager);
    public Task<RenderFragment?> Open(string workingDir, FsEntry entry, IFsAccess fsAccess, IFileManager fileManager);
}