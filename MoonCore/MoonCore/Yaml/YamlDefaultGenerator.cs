namespace MoonCore.Yaml;

public static class YamlDefaultGenerator
{
    public static async Task Generate<T>(string path)
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
}