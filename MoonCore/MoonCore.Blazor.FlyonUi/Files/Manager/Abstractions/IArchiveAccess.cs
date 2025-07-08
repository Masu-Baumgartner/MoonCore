namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IArchiveAccess
{
    public ArchiveFormat[] ArchiveFormats { get; }
    
    public Task Archive(string workingDir, FsEntry[] files, string path, ArchiveFormat format);
    public Task Unarchive(string workingDir, string path);
}