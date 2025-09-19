using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Extended.Helpers;

/// <summary>
/// Accessor for the service collection
/// </summary>
public class ServiceCollectionAccessor
{
    // We need this accessor as it is not possible to access the service descriptor collection
    // without a service with a reference to it. That's the reason why this small little helper exists
    
    public IServiceCollection ServiceCollection { get; private set; }
    
    public ServiceCollectionAccessor(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }
}