using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Extended.Extensions;

internal static class ServiceProviderExtensions
{
    /// <summary>
    /// Retrieves all DbContexts from a di service provider
    /// </summary>
    /// <param name="serviceProvider">Service provider to retrieve the DbContexts from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws if you forgot to call <see cref="Extensions.ServiceCollectionExtensions.AddServiceCollectionAccessor"/></exception>
    internal static Type[] GetDbContexts(this IServiceProvider serviceProvider)
    {
        var scAccessor = serviceProvider.GetService<ServiceCollectionAccessor>();

        if (scAccessor == null)
            throw new ArgumentException(
                "You need to call AddServiceCollectionAccessor() on the service collection in order to use this method");

        var dbContextType = typeof(DbContext);

        return scAccessor.ServiceCollection
            .Where(x => x.ServiceType.IsAssignableTo(dbContextType) && x.ServiceType != dbContextType)
            .Select(x => x.ServiceType)
            .ToArray();
    }
}