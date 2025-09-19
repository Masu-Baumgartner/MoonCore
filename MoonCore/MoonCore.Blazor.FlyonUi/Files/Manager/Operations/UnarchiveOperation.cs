using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class UnarchiveOperation : ISingleFsOperation
{
    public string Name => "Unarchive";
    public string Icon => "icon-archive-restore";
    public int Order => 0;

    public string ContextCss => "text-success";

    public Func<FsEntry, bool>? Filter => entry =>
    {
        return Formats.Any(format => format.Extensions.Any(extension => entry.Name.EndsWith(extension))
        );
    };

    private ArchiveFormat[] Formats = [];

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
    {
        if (access is not IArchiveAccess archiveAccess)
            return false;

        Formats = archiveAccess.ArchiveFormats;

        return true;
    }

    public Task ExecuteAsync(string workingDir, FsEntry entry, IFsAccess access, IFileManager fileManager)
    {
        throw new NotImplementedException();
    }
}