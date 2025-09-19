using System.Text;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.FlyonUi.Ace;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.OpenWindows;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class EditorOpenOperation : IFsOpenOperation
{
    public Func<FsEntry, bool> Filter => file => CodeEditorModeHelper.IsValidExtension(Path.GetExtension(file.Name));
    public int Order => 0;

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager) => true;

    public async Task<RenderFragment?> OpenAsync(
        string workingDir,
        FsEntry entry,
        IFsAccess fsAccess,
        IFileManager fileManager
    )
    {
        byte[]? buffer = null;

        var path = UnixPath.Combine(workingDir, entry.Name);

        await fsAccess.ReadAsync(
            path,
            async stream =>
            {
                buffer = new byte[stream.Length];
                await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
            }
        );

        if (buffer == null)
            return null;

        var content = Encoding.UTF8.GetString(buffer);

        return ComponentHelper.FromType<EditorWindow>(parameters =>
        {
            parameters["Content"] = content;
            parameters["FileName"] = entry.Name;

            parameters["OnSave"] = async (string newContent) =>
            {
                using var ms = new MemoryStream(
                    Encoding.UTF8.GetBytes(newContent)
                );

                await fsAccess.WriteAsync(path, ms);
            };

            parameters["OnClose"] = async () => await fileManager.CloseOpenScreenAsync();
        });
    }
}