using System.Text.Json;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.Services;

public class LocalStorageService
{
    private readonly IJSRuntime JsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task<bool> ContainsKey(string key)
    {
        return !string.IsNullOrEmpty(
            await JsRuntime.InvokeAsync<string?>("localStorage.getItem", key)
        );
    }

    #region Set

    public async Task SetString(string key, string data)
        => await JsRuntime.InvokeVoidAsync("localStorage.setItem", key, data);

    public async Task Set(string key, object data)
        => await SetString(key, JsonSerializer.Serialize(data));

    #endregion
    

    #region Get
    
    public async Task<string> GetString(string key)
        => await JsRuntime.InvokeAsync<string>("localStorage.getItem", key);

    public async Task<string> GetString(string key, string defaultValue)
    {
        if(!await ContainsKey(key))
            return defaultValue;

        return await GetString(key);
    }

    public async Task<T> Get<T>(string key, T defaultValue)
    {
        if(!await ContainsKey(key))
            return defaultValue;

        return await Get<T>(key);
    }
    
    public async Task<T> Get<T>(string key)
        => JsonSerializer.Deserialize<T>(await GetString(key))!;

    #endregion
}