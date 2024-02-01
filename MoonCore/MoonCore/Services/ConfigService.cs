using Newtonsoft.Json;

namespace MoonCore.Services;

public class ConfigService<T>
{
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
        Data = JsonConvert.DeserializeObject<T>(text) ?? Activator.CreateInstance<T>()!;
        
        ApplyEnvironmentVariables("Moonlight", Data);
        
        Save();
    }

    public void Save()
    {
        var text = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(Path, text);
    }

    public T Get()
    {
        return Data;
    }

    private void ApplyEnvironmentVariables(string prefix, object objectToLookAt)
    {
        foreach (var property in objectToLookAt.GetType().GetProperties())
        {
            var envName = $"{prefix}_{property.Name}";
            
            if (property.PropertyType.Assembly == GetType().Assembly)
            {
                ApplyEnvironmentVariables(envName, property.GetValue(objectToLookAt)!);
            }
            else
            {
                if(!Environment.GetEnvironmentVariables().Contains(envName))
                    continue;

                var envValue = Environment.GetEnvironmentVariable(envName)!;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(objectToLookAt, envValue);
                }
                else if (property.PropertyType == typeof(int))
                {
                    if(!int.TryParse(envValue, out int envIntValue))
                        continue;
                    
                    property.SetValue(objectToLookAt, envIntValue);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    if(!bool.TryParse(envValue, out bool envBoolValue))
                        continue;
                    
                    property.SetValue(objectToLookAt, envBoolValue);
                }
            }
        }
    }
}