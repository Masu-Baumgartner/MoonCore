using System.Text.Json;
using MoonCore.Helpers;

namespace MoonCore.Sse;

public class SseReader<T> : IAsyncEnumerable<SseItem<T>>
{
    private readonly StreamReader Reader;

    public SseReader(Stream stream)
    {
        Reader = new(stream);
    }

    public async IAsyncEnumerator<SseItem<T>> GetAsyncEnumerator(
        CancellationToken cancellationToken = new CancellationToken())
    {
        string? eventName = null;
        string? json = null;

        while (!cancellationToken.IsCancellationRequested && !Reader.EndOfStream)
        {
            var line = await Reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrEmpty(line))
            {
                // If data is complete, send the item over
                if (!string.IsNullOrEmpty(eventName) && !string.IsNullOrEmpty(json))
                {
                    // Construct item
                    var data = JsonSerializer.Deserialize<T>(json)!;

                    var item = new SseItem<T>()
                    {
                        Data = data,
                        Event = eventName
                    };

                    // Clear old data
                    eventName = null;
                    json = null;

                    yield return item;
                   
                }
                
                continue;
            }

            if (line.StartsWith("event: "))
            {
                eventName = Formatter.ReplaceStart(
                    line,
                    "event: ",
                    ""
                ).Trim();
            }
            else if (line.StartsWith("data: "))
            {
                json = Formatter.ReplaceStart(
                    line,
                    "data: ",
                    ""
                ).Trim();
            }
        }
        
        Reader.Close();
    }
}