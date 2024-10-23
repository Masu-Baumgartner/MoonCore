using Microsoft.JSInterop;

namespace MoonCore.Blazor.Services;

public class CookieService
{
    private readonly IJSRuntime JsRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    public async Task Set(string key, string value, int days)
    {
        var utc = DateTime.UtcNow.AddDays(days).ToUniversalTime().ToString("R");
        await SetCookies($"{key}={value}; expires={utc}; path=/");
    }

    public async Task<string> Get(string key, string def = "")
    {
        var cookieString = await GetCookies();

        var cookieParts = cookieString.Split(";");

        foreach (var cookiePart in cookieParts)
        {
            if(string.IsNullOrEmpty(cookiePart))
                continue;
            
            var cookieKeyValue = cookiePart.Split("=")
                .Select(x => x.Trim()) // There may be spaces e.g. with the "AspNetCore.Culture" key
                .ToArray();

            if (cookieKeyValue.Length == 2)
            {
                if (cookieKeyValue[0] == key)
                    return cookieKeyValue[1];
            }
        }
        
        return def;
    }

    #region Helpers

    private async Task SetCookies(string value)
    {
        await JsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
    }

    private async Task<string> GetCookies()
    {
        return await JsRuntime.InvokeAsync<string>("eval", "document.cookie");
    }

    #endregion
}