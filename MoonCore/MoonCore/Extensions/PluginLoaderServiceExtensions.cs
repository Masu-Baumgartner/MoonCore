using MoonCore.Helpers;
using MoonCore.Plugins;

namespace MoonCore.Extensions;

public static class PluginLoaderServiceExtensions
{
    public static void AddHttpHostedSource(this PluginLoaderService service, string url)
    {
        service.AddSource(new HttpHostedPluginSource(url));
    }

    public static void AddFolderSource(this PluginLoaderService service, string folder)
    {
        var entrypointsPath = PathBuilder.File(folder, "entrypoints.meta");

        if (!File.Exists(entrypointsPath))
        {
            throw new ArgumentException(
                "No entrypoints.meta in folder. Please create a entrypoints.meta in the folder so the plugin source is able to determine which assemblies to load as a plugin");
        }

        var entrypoints = File.ReadAllLines(entrypointsPath)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
        
        service.AddSource(new FolderPluginSource(folder, entrypoints));
    }
    
    public static void AddFolderSource(this PluginLoaderService service, string folder, string[] entrypoints)
    {
        service.AddSource(new FolderPluginSource(folder, entrypoints));
    }
    
    public static void AddFilesSource(this PluginLoaderService service, string[] files, string[] entrypoints)
    {
        service.AddSource(new FilesPluginSource(files, entrypoints));
    }
}