using Microsoft.Extensions.Configuration;


using var httpClient = new HttpClient();

Console.WriteLine("START");

var response = await httpClient.GetAsync("http://localhost:5220/api/stream", HttpCompletionOption.ResponseHeadersRead);

await using var stream = await response.Content.ReadAsStreamAsync();
using var reader = new StreamReader(stream);

while (!reader.EndOfStream)
{
    var line = await reader.ReadLineAsync();
    if (!string.IsNullOrWhiteSpace(line))
    {
        Console.WriteLine($"Received: {line}");
    }
}

Console.WriteLine("END");