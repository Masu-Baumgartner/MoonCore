using HeyRed.Mime;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.OpenWindows;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Operations;

public class ImageOpenOperation : IFsOpenOperation
{
    public Func<FsEntry, bool> Filter => entry => MimeTypesMap
        .GetMimeType(entry.Name)
        .StartsWith("image");
    
    public int Order => 0;

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager)
        => access is IDownloadUrlAccess;

    public async Task<RenderFragment?> OpenAsync(string workingDir, FsEntry entry, IFsAccess fsAccess, IFileManager fileManager)
    {
        var downloadUrlAccess = fsAccess as IDownloadUrlAccess;

        if (downloadUrlAccess == null)
            return null;
        
        var path = UnixPath.Combine(workingDir, entry.Name);

        var url = await downloadUrlAccess.GetFileUrlAsync(path);
        
        return ComponentHelper.FromType<ImageWindow>(parameters =>
        {
            parameters["Url"] = url;
            parameters["FileName"] = entry.Name;
            parameters["OnClose"] = async () => await fileManager.CloseOpenScreenAsync();
        });
    }
}