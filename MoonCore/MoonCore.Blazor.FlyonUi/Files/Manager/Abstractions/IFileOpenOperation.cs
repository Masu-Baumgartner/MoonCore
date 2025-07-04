using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFileOpenOperation
{
    public Func<FileEntry, bool> Filter { get; set; }
    public int Order { get; }
    public Task<RenderFragment> Open(string workingDir, FileEntry entry, IFileAccess fileAccess, IFileManager fileManager);
}