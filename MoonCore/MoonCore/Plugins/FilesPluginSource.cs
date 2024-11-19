using System.Runtime.Loader;

namespace MoonCore.Plugins;

public class FilesPluginSource : IPluginSource
{
    public string[] AssemblyFiles { get; set; }
    public string[] Entrypoints { get; set; }
    
    public FilesPluginSource(string[] assemblyFiles, string[] entrypoints)
    {
        AssemblyFiles = assemblyFiles;
        Entrypoints = entrypoints;
    }
    
    public Task Load(AssemblyLoadContext loadContext, List<string> entrypoints)
    {
        foreach (var assemblyFile in AssemblyFiles)
        {
            var fs = File.OpenRead(assemblyFile);
            
            loadContext.LoadFromStream(fs);
            
            fs.Close();
        }
        
        entrypoints.AddRange(Entrypoints);
        
        return Task.CompletedTask;
    }
}