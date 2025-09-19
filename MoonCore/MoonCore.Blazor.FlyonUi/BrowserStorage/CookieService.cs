using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.BrowserStorage;

/// <summary>
/// Interop service to provider access to the browsers cookies
/// </summary>
public class CookieService
{
    // TODO: Consider replacing with the CookieStore
    // https://developer.mozilla.org/en-US/docs/Web/API/CookieStore
    
    private readonly IJSRuntime JsRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Sets or overrides a specific cookie with the provided value
    /// </summary>
    /// <param name="key">Key of the cookie</param>
    /// <param name="value">Value to set the cookie to</param>
    /// <param name="days">Days until the browser should delete the cookie</param>
    public async Task SetAsync(string key, string value, int days)
    {
        var utc = DateTime.UtcNow.AddDays(days).ToUniversalTime().ToString("R");
        var cookie = $"{key}={value}; expires={utc}; path=/";
        
        await JsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{cookie}\"");
    }

    /// <summary>
    /// Retrieves the value of a cookie by its key.
    /// </summary>
    /// <param name="key">The name of the cookie to retrieve.</param>
    /// <param name="fallbackValue">
    /// A value to return if the cookie with the specified key does not exist.
    /// Defaults to an empty string.
    /// </param>
    /// <returns>
    /// The value of the cookie if found; otherwise, the <paramref name="fallbackValue"/>.
    /// </returns>
    public async Task<string> GetAsync(string key, string fallbackValue = "")
    {
        var cookieString = await JsRuntime.InvokeAsync<string>("eval", "document.cookie");

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
        
        return fallbackValue;
    }
}