using Microsoft.Extensions.Configuration;
using MoonCore.Sse;
using MoonCore.Test;


using var httpClient = new HttpClient();

Console.WriteLine("START");

var response = await httpClient.GetAsync("http://localhost:5220/api/stream", HttpCompletionOption.ResponseHeadersRead);

await using var stream = await response.Content.ReadAsStreamAsync();
var sseReader = new SseReader<Testy>(stream);

await foreach (var item in sseReader)
{
    Console.WriteLine(item.Data.Counter);
}