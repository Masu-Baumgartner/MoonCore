using MoonCore.Blazor.Tailwind.Fm.Models;

namespace MoonCore.Blazor.Tailwind.Fm;

public interface ICompressFileSystemProvider
{
    public CompressType[] CompressTypes { get; }
    
    public Task Compress(CompressType type, string path, string[] itemsToCompress);
    public Task Decompress(CompressType type, string path, string destination);
}