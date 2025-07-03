using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public partial class FileManager
{
    private async Task RunToolbarOperation(IToolbarOperation operation)
    {
        var workingDir = new string(CurrentPath);

        await operation.Execute(workingDir, FileAccess, this);
    }
}