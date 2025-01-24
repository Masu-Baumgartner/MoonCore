using MoonCore.Blazor.Tailwind.Test.UI.Fm.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    private ICompressFileSystemProvider? CompressProvider
    {
        get
        {
            if (HasCheckedProvider)
                return CachedProviderResult;

            CachedProviderResult = FileSystemProvider as ICompressFileSystemProvider;
            HasCheckedProvider = true;

            return CachedProviderResult;
        }
    }

    private bool HasCheckedProvider = false;
    private ICompressFileSystemProvider? CachedProviderResult;

    private async Task Compress(FileSystemEntry[] entries)
    {
        if(CompressProvider == null)
            return;
        
        async Task InternalCompress(CompressType compressType, string fileName)
        {
            await CompressProvider.Compress(
                compressType,
                PathBuilder.JoinPaths(CurrentPath, fileName),
                entries.Select(x => x.Name).ToArray()
            );

            await ToastService.Success("Successfully created archive", fileName);
            await FileView.Refresh();
        }
        
        await ModalService.Launch<CompressFileNameModal>(parameters =>
        {
            parameters.Add("OnSubmit", InternalCompress);
            parameters.Add("CompressTypes", CompressProvider.CompressTypes);
        });
    }
}