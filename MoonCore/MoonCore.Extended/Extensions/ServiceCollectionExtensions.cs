using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.ExceptionHandlers;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.SingleDb;

namespace MoonCore.Extended.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApiExceptionHandler(this IServiceCollection collection)
    {
        collection.AddProblemDetails();
        collection.AddExceptionHandler<ApiExceptionHandler>();
    }

    public static void AddDatabaseMappings(this IServiceCollection collection)
    {
        collection.AddSingleton<DatabaseMappingOptions>();
    }

    public static void AddDatabaseContext<T>(this IServiceCollection collection, ServiceLifetime lifetime) where T : DatabaseContext
    {
        collection.Add(new ServiceDescriptor(typeof(DatabaseContext), typeof(T), lifetime));
        collection.AddDbContext<DbContext, T>(lifetime);
    }
}