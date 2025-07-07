using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.ExceptionHandlers;
using MoonCore.Extended.Helpers;

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

    public static void AddServiceCollectionAccessor(this IServiceCollection collection)
    {
        collection.AddSingleton(
            new ServiceCollectionAccessor(collection)
        );
    }
}