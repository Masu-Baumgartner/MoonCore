using MoonCore.Blazor.Tailwind.Test.UI.Fm.Models;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public interface ICompressFileSystemProvider
{
    public CompressType[] CompressTypes { get; }
    
    public Task Compress(CompressType type, string path, string[] itemsToArchive);
    public Task Decompress(CompressType type, string path, string destination);
}