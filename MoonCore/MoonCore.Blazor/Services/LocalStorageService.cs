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
        => await JsRuntime.InvokeAsync<string?>("localStorage.getItem", key) != null;

    public async Task<string> Get(string key)
        => await Get<string>(key);
    
    public async Task<string> Get(string key, string defaultValue)
        => await Get<string?>(key) ?? defaultValue;

    public async Task<T> Get<T>(string key)
        => JsonSerializer.Deserialize<T>(await Get(key))!;

    public async Task<T> Get<T>(string key, T defaultValue)
    {
        var val = await Get<string?>(key);

        if (val == null)
            return defaultValue;

        return JsonSerializer.Deserialize<T>(val) ?? defaultValue;
    }

    public async Task SetString(string key, string data)
        => await JsRuntime.InvokeVoidAsync("localStorage.setItem", key, data);

    public async Task Set(string key, object data)
        => await SetString(key, JsonSerializer.Serialize(data));
}