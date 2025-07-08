using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MoonCore.Sse;

namespace MoonCore.Extended.Sse;

public class SseResult<T> : IResult
{
    private readonly IAsyncEnumerable<SseItem<T>> Provider;

    public SseResult(IAsyncEnumerable<SseItem<T>> provider)
    {
        Provider = provider;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var response = httpContext.Response;

        response.ContentType = "text/event-stream";
        
        await foreach (var item in Provider)
        {
            Console.WriteLine("TEST");
            
            var eventName = item.Event ?? "data";
            var json = JsonSerializer.Serialize(item.Data);
            
            await response.WriteAsync($"event: {eventName}\n");
            await response.WriteAsync($"data: {json}\n");
            await response.WriteAsync("\n");
            
            await response.Body.FlushAsync();
            
            Console.WriteLine("FLUSH");
        }
    }
}