using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Helpers;
using MoonCore.PluginFramework.Configuration;

namespace MoonCore.PluginFramework.Services;

public class InterfaceService
{
    private readonly InterfaceConfiguration Configuration;
    private readonly ILogger Logger;

    public InterfaceService(InterfaceConfiguration configuration, ILogger logger)
    {
        Configuration = configuration;
        Logger = logger;
    }

    public void ConstructInterfaces(IServiceCollection serviceCollection)
    {
        // Collect all types
        var types = Configuration.Assemblies
            .SelectMany(x => x.ExportedTypes)
            .ToArray();

        Logger.LogTrace("Loaded {amount} types from {assemblyAmount} assemblies", types.Length,
            Configuration.Assemblies.Count);

        // Process Interfaces
        foreach (var interfaceKvp in Configuration.Interfaces)
        {
            // Get all types implementing this interface
            var implementations = types
                .Where(x => x.IsAssignableTo(interfaceKvp.Key))
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .ToArray();

            foreach (var implementation in implementations) // Register interfaces to the di
            {
                serviceCollection.Add(new ServiceDescriptor(
                    implementation,
                    sp => ActivatorUtilities.CreateInstance(sp, implementation),
                    interfaceKvp.Value
                ));
            }

            // Build descriptor for those interfaces
            var descriptor = new ServiceDescriptor(
                interfaceKvp.Key.MakeArrayType(),
                sp => ReflectionHelper.InvokeGenericMethod(
                    null,
                    GetType(),
                    nameof(ResolveTypesFromDi),
                    [interfaceKvp.Key], [
                        sp, implementations, interfaceKvp.Key, Logger
                    ])!,
                interfaceKvp.Value
            );

            // Add it to the collection
            serviceCollection.Add(descriptor);

            Logger.LogTrace("Registered interface {name} to dependency injection", interfaceKvp.Key.FullName);
        }
    }

    private static T[] ResolveTypesFromDi<T>(
        IServiceProvider provider,
        Type[] types,
        Type interfaceType,
        ILogger logger
    )
    {
        var result = new List<T>();

        foreach (var type in types)
        {
            try
            {
                var interfaceImplInstance = provider.GetService(type);

                if (interfaceImplInstance == null)
                {
                    logger.LogCritical(
                        "Unable to resolve {implName} for {intName} from di",
                        type.FullName ?? "unknown",
                        interfaceType.FullName ?? "unknown"
                    );

                    continue;
                }

                result.Add((T)interfaceImplInstance);
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    "An unhandled error occured while constructing implementation '{implName}' for interface '{intName}': {e}",
                    type.FullName ?? "unknown",
                    interfaceType.FullName ?? "unknown",
                    e
                );
            }
        }

        return result.ToArray();
    }
}