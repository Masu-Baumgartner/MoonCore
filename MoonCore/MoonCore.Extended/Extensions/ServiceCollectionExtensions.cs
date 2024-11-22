using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.ExceptionHandlers;

namespace MoonCore.Extended.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApiExceptionHandler(this IServiceCollection collection)
    {
        collection.AddProblemDetails();
        collection.AddExceptionHandler<ApiExceptionHandler>();
    }
}