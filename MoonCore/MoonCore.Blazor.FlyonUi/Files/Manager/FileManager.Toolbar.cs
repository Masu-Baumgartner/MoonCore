using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private async Task RunToolbarOperationAsync(IToolbarOperation operation)
    {
        var workingDir = new string(CurrentPath);

        await operation.ExecuteAsync(workingDir, FsAccess, this);
    }
}