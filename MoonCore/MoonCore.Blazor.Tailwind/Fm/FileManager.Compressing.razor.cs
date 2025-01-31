using MoonCore.Blazor.Tailwind.Fm.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    private ICompressFileSystemProvider? CompressProvider;

    private async Task Compress(FileSystemEntry[] entries)
    {
        // Do nothing if we have no provider for it
        if (CompressProvider == null)
            return;

        async Task InternalCompress(CompressType compressType, string fileName)
        {
            await ToastService.Progress(
                "Compressing",
                "Please be patient",
                async _ =>
                {
                    await CompressProvider.Compress(
                        compressType,
                        PathBuilder.JoinPaths(CurrentPath, fileName),
                        entries.Select(x => PathBuilder.JoinPaths(CurrentPath, x.Name)).ToArray()
                    );

                    await ToastService.Success("Successfully created archive", fileName);
                    
                    // Reset state
                    await SetAllSelection(false);
                    await FileList.Refresh();
                }
            );
        }

        await ModalService.Launch<CompressFileNameModal>(parameters =>
        {
            parameters.Add("OnSubmit", InternalCompress);
            parameters.Add("CompressTypes", CompressProvider.CompressTypes);
        });
    }

    // This overload is unused atm
    private async Task Decompress(FileSystemEntry entry)
    {
        // Do nothing if we have no provider for it
        if (CompressProvider == null)
            return;
        
        var compressType = CompressProvider.CompressTypes.FirstOrDefault(
            x => entry.Name.EndsWith(x.Extension, StringComparison.InvariantCultureIgnoreCase)
        );

        if (compressType == null) // Because th ui only shows the option to extract if its supported, this should never be true
        {
            await ToastService.Danger("Unable to extract: Unsupported archive type");
            return;
        }

        await Decompress(entry, compressType);
    }
    
    private async Task Decompress(FileSystemEntry entry, CompressType compressType)
    {
        // Do nothing if we have no provider for it
        if(CompressProvider == null)
            return;
        
        async Task InternalDecompress(string destination)
        {
            await ToastService.Progress(
                "Decompressing",
                "Please be patient",
                async _ =>
                {
                    await CompressProvider.Decompress(
                        compressType,
                        PathBuilder.JoinPaths(CurrentPath, entry.Name),
                        destination
                    );

                    await ToastService.Success("Successfully extracted archive", entry.Name);
                    
                    // Reset state
                    await SetAllSelection(false);
                    await FileList.Refresh();
                }
            );
        }
        
        await ModalService.Launch<LocationSelectModal>(parameters =>
        {
            parameters.Add("OnSubmit", InternalDecompress);
            parameters.Add("InitialPath", CurrentPath);
            parameters.Add("FileSystemProvider", FileSystemProvider);
        });
    }
}