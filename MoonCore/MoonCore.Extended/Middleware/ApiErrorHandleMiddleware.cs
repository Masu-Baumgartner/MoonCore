using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Exceptions;

namespace MoonCore.Extended.Middleware;

public class ApiErrorHandleMiddleware
{
    private readonly RequestDelegate Next;

    public ApiErrorHandleMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch (HttpApiException httpApiException)
        {
            await Results.Problem(
                title: httpApiException.Title,
                detail: httpApiException.Detail,
                statusCode: httpApiException.Status,
                type: "general-api-error"
            ).ExecuteAsync(context);
        }
        catch (HttpRequestException e)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ApiErrorHandleMiddleware>>();
            
            if (e.InnerException is SocketException)
            {
                logger.LogCritical("An unhandled socket exception occured. [{method}] {path}: {e}", context.Request.Method, context.Request.Path, e.Demystify());
            
                await Results.Problem(
                    title: "An socket exception occured on the api server",
                    detail: "Check the api server logs for more details",
                    statusCode: 502,
                    type: "remote-api-connection-error"
                ).ExecuteAsync(context);
                
                return;
            }
            
            logger.LogCritical("An unhandled exception occured. [{method}] {path}: {e}", context.Request.Method, context.Request.Path, e.Demystify());
            
            await Results.Problem(
                title: "An http request exception occured on the api server",
                detail: "Check the api server logs for more details",
                statusCode: 500,
                type: "remote-api-request-error"
            ).ExecuteAsync(context);
        }
        catch (Exception e)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ApiErrorHandleMiddleware>>();
            
            logger.LogCritical("An unhandled exception occured. [{method}] {path}: {e}", context.Request.Method, context.Request.Path, e.Demystify());
            
            await Results.Problem(
                title: "An unhanded exception occured on the api server",
                detail: "Check the api server logs for more details",
                statusCode: 500,
                type: "critical-api-error"
            ).ExecuteAsync(context);
        }
    }
}