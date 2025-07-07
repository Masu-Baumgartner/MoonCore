using System.Text.Json;
using MoonCore.Exceptions;
using MoonCore.Models;

namespace MoonCore.Extensions;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions Options = new()
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
        if (response.IsSuccessStatusCode)
            return;

        var responseText = await response.Content.ReadAsStringAsync();
        
        try
        {
            var model = JsonSerializer.Deserialize<HttpApiErrorModel>(responseText, Options);

            if (model == null)
            {
                throw new HttpApiException(
                    "An HTTP API error occured",
                    (int)response.StatusCode,
                    responseText
                );
            }

            throw new HttpApiException(model.Title, model.Status, model.Detail, model.Errors);
        }
        catch (JsonException)
        {
            throw new HttpApiException(
                "An HTTP API error occured",
                (int)response.StatusCode,
                responseText
            );
        }
    }
}