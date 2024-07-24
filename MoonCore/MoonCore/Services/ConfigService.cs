using System.Text.Json;

namespace MoonCore.Services;

public class ConfigService<T>
{
    private JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
    
    private readonly string Path;
    private T Data;

    public ConfigService(string path)
    {
        Path = path;
        
        Reload();
    }

    public void Reload()
    {
        if(!File.Exists(Path))
            File.WriteAllText(Path, "{}");

        var text = File.ReadAllText(Path);
        Data = JsonSerializer.Deserialize<T>(text, SerializerOptions) ?? Activator.CreateInstance<T>()!;
        
        Save();
    }

    public void Save()
    {
        var text = JsonSerializer.Serialize(Data, SerializerOptions);
        File.WriteAllText(Path, text);
    }

    public T Get()
    {
        return Data;
    }
}