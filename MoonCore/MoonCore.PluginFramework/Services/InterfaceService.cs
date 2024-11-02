using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Helpers;
using MoonCore.PluginFramework.Configuration;

namespace MoonCore.PluginFramework.Services;

public class InterfaceService
{
    public void RegisterInterfaces(InterfaceConfiguration configuration, IServiceCollection serviceCollection)
    {
        // Collect all types
        var types = configuration.Assemblies
            .SelectMany(x => x.ExportedTypes)
            .ToArray();

        // Process Interfaces
        foreach (var interfaceKvp in configuration.Interfaces)
        {
            // Get all types implementing this interface
            var implementations = types
                .Where(x => x.IsAssignableTo(interfaceKvp.Type))
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .ToArray();

            foreach (var implementation in implementations) // Register interfaces to the di
            {
                serviceCollection.Add(new ServiceDescriptor(
                    implementation,
                    sp => ActivatorUtilities.CreateInstance(sp, implementation),
                    interfaceKvp.Scope
                ));
            }

            // Build descriptor for those interfaces
            var descriptor = new ServiceDescriptor(
                interfaceKvp.Type.MakeArrayType(),
                sp =>
                {
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(interfaceKvp.Type);

                    return ReflectionHelper.InvokeGenericMethod(
                        null,
                        GetType(),
                        nameof(ResolveTypesFromDi),
                        [interfaceKvp.Type], [
                            sp, implementations, interfaceKvp.Type, logger
                        ])!;
                },
                interfaceKvp.Scope
            );

            // Add it to the collection
            serviceCollection.Add(descriptor);
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