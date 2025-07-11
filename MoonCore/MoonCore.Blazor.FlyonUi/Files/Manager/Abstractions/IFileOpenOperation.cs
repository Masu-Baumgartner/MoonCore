using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFileOpenOperation
{
    public Func<FsEntry, bool> Filter { get; set; }
    public int Order { get; }
    public Task<RenderFragment> Open(string workingDir, FsEntry entry, IFsAccess fsAccess, IFileManager fileManager);
}