using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Exceptions;

namespace MoonCore.Extended.ExceptionHandlers;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is HttpApiException httpApiException)
        {
            await Results.Problem(
                title: httpApiException.Title,
                detail: httpApiException.Detail,
                statusCode: httpApiException.Status,
                type: "general-api-error"
            ).ExecuteAsync(httpContext);
        }
        else
        {
            var logger = httpContext.RequestServices.GetRequiredService<ILogger<ApiExceptionHandler>>();
            var method = httpContext.Request.Method;
            var path = httpContext.Request.Path;

            if (exception is HttpRequestException httpRequestException)
            {
                if (httpRequestException.InnerException is SocketException)
                {
                    logger.LogCritical(
                        "An unhandled socket exception occured. [{method}] {path}: {e}",
                        method,
                        path,
                        exception.Demystify()
                    );

                    await Results.Problem(
                        title: "An socket exception occured on the api server",
                        detail: "Check the api server logs for more details",
                        statusCode: 502,
                        type: "remote-api-connection-error"
                    ).ExecuteAsync(httpContext);
                }
                else
                {
                    logger.LogCritical(
                        "An unhandled exception occured. [{method}] {path}: {e}",
                        method,
                        path,
                        exception.Demystify()
                    );

                    await Results.Problem(
                        title: "An http request exception occured on the api server",
                        detail: "Check the api server logs for more details",
                        statusCode: 500,
                        type: "remote-api-request-error"
                    ).ExecuteAsync(httpContext);
                }
            }
            else
            {
                logger.LogCritical(
                    "An unhandled exception occured. [{method}] {path}: {e}",
                    method,
                    path,
                    exception.Demystify()
                );

                await Results.Problem(
                    title: "An unhanded exception occured on the api server",
                    detail: "Check the api server logs for more details",
                    statusCode: 500,
                    type: "critical-api-error"
                ).ExecuteAsync(httpContext);
            }
        }

        return true;
    }
}