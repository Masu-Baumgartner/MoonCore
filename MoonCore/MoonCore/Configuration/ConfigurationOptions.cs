using MoonCore.Helpers;

namespace MoonCore.Configuration;

// A weird name, ik ik
public class ConfigurationOptions
{
    public List<ConfigurationOption> ConfigurationTypes { get; set; } = new();
    public string Path { get; set; } = "configs";
    public string EnvironmentPrefix { get; set; } = "APP";

    public void AddConfiguration<T>(string? name = null)
    {
        var configType = typeof(T);

        ConfigurationTypes.Add(new()
        {
            Name = name ?? configType.Name,
            Type = configType
        });
    }

    public void UsePath(string path)
        => Path = path;
    
    public void UseEnvironmentPrefix(string prefix)
        => EnvironmentPrefix = prefix;

    public class ConfigurationOption
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}