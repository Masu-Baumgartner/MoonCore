using System.Runtime.Loader;

namespace MoonCore.Plugins;

public class FolderPluginSource : IPluginSource
{
    private readonly string Folder;
    private readonly string[] Entrypoints;

    public FolderPluginSource(string folder, string[] entrypoints)
    {
        Folder = folder;
        Entrypoints = entrypoints;
    }

    public async Task Load(AssemblyLoadContext loadContext, List<string> entrypoints)
    {
        var dlls = new List<string>();
        
        GetDllsInFolder(dlls, Folder);

        foreach (var dll in dlls)
            loadContext.LoadFromStream(File.OpenRead(dll));
        
        entrypoints.AddRange(Entrypoints);
    }

    private void GetDllsInFolder(List<string> output, string folder)
    {
        output.AddRange(Directory.EnumerateFiles(folder).Where(x => x.EndsWith(".dll")));

        foreach (var directory in Directory.EnumerateDirectories(folder))
            GetDllsInFolder(output, directory);
    }
}