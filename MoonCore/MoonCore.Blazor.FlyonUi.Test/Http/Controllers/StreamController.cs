using Microsoft.AspNetCore.Mvc;
using MoonCore.Extended.Sse;
using MoonCore.Sse;

namespace MoonCore.Blazor.FlyonUi.Test.Http.Controllers;

[ApiController]
[Route("api/stream")]
public class StreamController : Controller
{
    private int Counter = 0;
    
    [HttpGet]
    public IResult Index()
    {
        async IAsyncEnumerable<SseItem<Testy>> GetTesty(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                yield return new SseItem<Testy>("testy")
                {
                    Data = new()
                    {
                        Counter = Counter++
                    }
                };
            }
        }

        return SseResults.ServerSideEvents(GetTesty(HttpContext.RequestAborted));
    }

    record Testy
    {
        public int Counter { get; set; }
    }
}