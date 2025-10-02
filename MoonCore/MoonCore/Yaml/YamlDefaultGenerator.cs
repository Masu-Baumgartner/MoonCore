namespace MoonCore.Yaml;

/// <summary>
/// Helper for generating / updating a yaml file in a specific scheme
/// </summary>
public static class YamlDefaultGenerator
{
    /// <summary>
    /// Generates a yaml file in a specific scheme using the defined default values.
    /// If a file already exists it updates 
    /// </summary>
    /// <param name="path">Path of the yaml file</param>
    /// <typeparam name="T">Type of the scheme</typeparam>
    public static async Task GenerateAsync<T>(string path)
    {
        T model;

        if (File.Exists(path))
        {
            var yaml = await File.ReadAllTextAsync(path);
            model = YamlSerializer.Deserialize<T>(yaml);
        }
        else
            model = Activator.CreateInstance<T>();

        var newYaml = YamlSerializer.Serialize(model);
        await File.WriteAllTextAsync(path, newYaml);
    }
    
    /// <summary>
    /// Generates a yaml file in a specific scheme using the defined default values.
    /// If a file already exists it updates 
    /// </summary>
    /// <param name="path">Path of the yaml file</param>
    /// <typeparam name="T">Type of the scheme</typeparam>
    public static void Generate<T>(string path)
    {
        T model;

        if (File.Exists(path))
        {
            var yaml = File.ReadAllText(path);
            model = YamlSerializer.Deserialize<T>(yaml);
        }
        else
            model = Activator.CreateInstance<T>();

        var newYaml = YamlSerializer.Serialize(model);
        File.WriteAllText(path, newYaml);
    }
}