using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.ExceptionHandlers;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.Services;

namespace MoonCore.Extended.Extensions;

/// <summary>
/// Extension methods to registered mooncore extended services with one method call
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an exception handler which handles exceptions and returns an RFC 9457 problem detail as the response
    /// Follow <see href="https://www.rfc-editor.org/rfc/rfc9457.html">this</see> link for more details on the RFC
    /// </summary>
    /// <param name="collection"></param>
    public static void AddApiExceptionHandler(this IServiceCollection collection)
    {
        collection.AddProblemDetails();
        collection.AddExceptionHandler<ApiExceptionHandler>();
    }

    /// <summary>
    /// Adds the storage options for the database mapping required by the <see cref="DatabaseRepository{TEntity}"/>
    /// </summary>
    /// <param name="collection"></param>
    public static void AddDatabaseMappings(this IServiceCollection collection)
    {
        collection.TryAddSingleton(new ServiceCollectionAccessor(collection));
        collection.AddSingleton<DatabaseMappingOptions>();
        collection.AddHostedService<DatabaseMappingsService>();
    }

    /// <summary>
    /// Adds a service which allows DI consumers to retrieve the list of registered services.
    /// Used to generate DbContext - Entity mappings
    /// </summary>
    /// <param name="collection"></param>
    public static void AddServiceCollectionAccessor(this IServiceCollection collection)
    {
        collection.TryAddSingleton(new ServiceCollectionAccessor(collection));
    }

    /// <summary>
    /// Adds a service which automatically applies all pending migrations of all registered
    /// <see cref="Microsoft.EntityFrameworkCore.DbContext"/> instances
    /// </summary>
    /// <param name="collection">Service collection to register the service to</param>
    public static void AddDbAutoMigrations(this IServiceCollection collection)
    {
        collection.TryAddSingleton(new ServiceCollectionAccessor(collection));
        collection.AddHostedService<DatabaseMigrationService>();
    }
}