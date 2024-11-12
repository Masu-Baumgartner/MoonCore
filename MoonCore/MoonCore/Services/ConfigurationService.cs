using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Configuration;
using MoonCore.Helpers;

namespace MoonCore.Services;

public class ConfigurationService
{
    private readonly JsonSerializerOptions SerializerOptions;
    private readonly Dictionary<Type, object> ConfigurationCache = new();

    public ConfigurationService()
    {
        SerializerOptions = new()
        {
            WriteIndented = true
        };
    }

    public void RegisterInDi(ConfigurationOptions options, IServiceCollection collection)
    {
        foreach (var option in options.Definitions)
        {
            collection.Add(new ServiceDescriptor(
                    option.Type,
                    sp =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger(option.Name);

                        return ReflectionHelper.InvokeGenericMethod(
                            this,
                            GetType(),
                            "GetConfiguration",
                            [option.Type],
                            [options, logger]
                        )!;
                    },
                    ServiceLifetime.Transient
                )
            );
        }
    }

    public T GetConfiguration<T>(ConfigurationOptions options, ILogger logger)
    {
        var configurationType = typeof(T);
        
        // If cached, return the cached object
        if (ConfigurationCache.TryGetValue(configurationType, out var cachedConfig))
            return (T)cachedConfig;

        // Find definition
        var definition = options.Definitions.FirstOrDefault(x => x.Type == configurationType);

        if (definition == null)
            throw new ArgumentException($"No definition of {configurationType.FullName} found in the provided options");
        
        // First, read the config file, create it if it doesn't exist
        var configPath = PathBuilder.File(options.Path, definition.Name + ".json");
        
        if (!File.Exists(configPath)) // Create config file if not existing
        {
            logger.LogTrace("Configuration file for '{name}' not found. Creating it", definition.Name);

            try
            {
                File.WriteAllText(configPath, "{}");
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    "An unhandled error occured while creating a missing configuration file '{pth}': {e}", configPath,
                    e);
                throw;
            }
        }

        // Read file
        var configurationText = File.ReadAllText(configPath);

        // Now we parse the json contents
        T deserializedConfiguration;

        try
        {
            deserializedConfiguration = JsonSerializer.Deserialize<T>(configurationText, SerializerOptions)!;
        }
        catch (Exception e)
        {
            logger.LogCritical("An unhandled error occured while parsing the configuration file for in '{path}': {e}",
                configPath, e);
            throw;
        }

        // To automatically save new values for options which haven't existed before
        // we serialize the config model right after reading it and save the new version if changes
        // have been detected

        var serializedConfig = JsonSerializer.Serialize(deserializedConfiguration, SerializerOptions);

        // Detect changes, to trigger backup function
        if (configurationText.Trim() != serializedConfig.Trim())
        {
            if (configurationText != "{}")
            {
                logger.LogTrace("Detected migration changes on configuration file {path}. Creating a backup", configPath);

                try
                {
                    var backupPath = configPath + "." + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    File.WriteAllText(backupPath, configurationText);
                }
                catch (Exception e)
                {
                    logger.LogCritical("An unhandled error occured while creating a configuration backup for '{path}': {e}",
                        configPath, e);
                    throw;
                }
            }

            // Now we can save the new version
            File.WriteAllText(configPath, serializedConfig);
        }

        // Handle environment variable override
        var prefix = $"{options.EnvironmentPrefix}_{definition.Name.ToUpper()}_";

        var possibleEnvs = Environment
            .GetEnvironmentVariables()
            .Keys
            .Cast<string>()
            .Where(x => x.StartsWith(prefix))
            .ToArray();

        foreach (var possibleEnv in possibleEnvs)
        {
            var path = Formatter.ReplaceStart(possibleEnv, prefix, "");

            var propertyData = ResolveProperty(
                configurationType,
                deserializedConfiguration,
                path.Split("_")
            );

            if (propertyData == null)
            {
                logger.LogWarning("Ignoring environment set override '{ignoredEnv}' for configuration '{name}'",
                    possibleEnv, definition.Name);
                continue;
            }

            var envValue = Environment.GetEnvironmentVariable(possibleEnv);

            if (string.IsNullOrEmpty(envValue))
            {
                logger.LogWarning(
                    "Ignoring environment set override '{ignoredEnv}' for configuration '{name}' as it was empty/unset",
                    possibleEnv, definition.Name);
                continue;
            }

            var property = propertyData.Value.Item1;
            var instance = propertyData.Value.Item2;

            try
            {
                if (property.PropertyType == typeof(int))
                    property.SetValue(instance, int.Parse(envValue));
                else if (property.PropertyType == typeof(string))
                    property.SetValue(instance, envValue);
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(instance, bool.Parse(envValue));
                else
                {
                    logger.LogWarning("Unsupported property type '{type}' for environment variable: {envVar}",
                        property.PropertyType.Name, possibleEnv);
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    "An unhandled error occured while applying environment value to configuration model. Environment variable: {variable}: {e}",
                    possibleEnv, e);
                throw;
            }
        }
        
        // Save it to the cache
        ConfigurationCache.Add(configurationType, deserializedConfiguration);

        return deserializedConfiguration;
    }

    private static (PropertyInfo, object)? ResolveProperty(Type type, object instance, string[] pathParts)
    {
        var lastType = type;
        var lastInstance = instance;

        for (var i = 0; i < pathParts.Length; i++)
        {
            var currentProperty = lastType
                .GetProperties()
                .FirstOrDefault(
                    x => x.Name.Equals(pathParts[i], StringComparison.CurrentCultureIgnoreCase)
                );

            if (currentProperty == null)
                return null;

            if (i == pathParts.Length - 1)
                return (currentProperty, lastInstance);

            lastType = currentProperty.PropertyType;
            lastInstance = currentProperty.GetValue(lastInstance)!;
        }

        return null;
    }
}