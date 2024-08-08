using System.Net.Http.Json;

namespace MoonCore.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> PostAsJsonReceiveJson<T>(this HttpClient client, string endpoint, object data)
    {
        var response = await client.PostAsJsonAsync(endpoint, data);

        await response.HandlePossibleApiError();
        
        return await response.ParseAsJson<T>();
    }
    
    public static async Task<T> PatchAsJsonReceiveJson<T>(this HttpClient client, string endpoint, object data)
    {
        var response = await client.PatchAsJsonAsync(endpoint, data);
        
        await response.HandlePossibleApiError();
        
        return await response.ParseAsJson<T>();
    }
}