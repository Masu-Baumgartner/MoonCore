using System.Text.Json;
using MoonCore.Exceptions;
using MoonCore.Models;

namespace MoonCore.Extensions;

public static class HttpResponseMessageExtensions
{
    private static JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public static async Task<T> ParseAsJson<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, Options)!;
    }

    public static async Task HandlePossibleApiError(this HttpResponseMessage response)
    {
        if(response.IsSuccessStatusCode)
            return;

        var model = await response.ParseAsJson<HttpApiErrorModel>();

        throw new HttpApiException(model.Title, model.Status, model.Detail, model.Errors);
    }
}