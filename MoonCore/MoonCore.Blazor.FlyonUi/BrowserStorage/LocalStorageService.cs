using System.Text.Json;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.BrowserStorage;

/// <summary>
/// Interop service class which provides access to the browsers local storage
/// </summary>
public class LocalStorageService
{
    private readonly IJSRuntime JsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    /// <summary>
    /// Checks if the local storage contains a specific key
    /// </summary>
    /// <param name="key">Key to search for</param>
    /// <returns>True if it exists and is not empty, false if it doesn't exist, or it is empty</returns>
    public async Task<bool> ContainsKeyAsync(string key)
    {
        var value = await GetStringAsync(key);
        
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Saves a string value with a specific key
    /// </summary>
    /// <param name="key">Key to save the string with</param>
    /// <param name="data">String to save to local storage</param>
    public async Task SetStringAsync(string key, string data)
        => await JsRuntime.InvokeVoidAsync("localStorage.setItem", key, data);

    /// <summary>
    /// Saves a object to local storage by serializing it
    /// </summary>
    /// <param name="key">Key to save the object with</param>
    /// <param name="data">Object to save to local storage</param>
    public async Task SetAsync(string key, object data)
        => await SetStringAsync(key, JsonSerializer.Serialize(data));
    
    /// <summary>
    /// Retrieves the string value for a specific key
    /// </summary>
    /// <param name="key">Key of the value to retrieve</param>
    /// <returns>String value if exists in local storage. If it doesn't exist the string may be null or empty</returns>
    public async Task<string> GetStringAsync(string key)
        => await JsRuntime.InvokeAsync<string>("localStorage.getItem", key);

    /// <summary>
    /// Retrieves the string value for a specific key with a fallback value if the local storage doesn't contain
    /// any value with that key
    /// </summary>
    /// <param name="key">Key of the value to retrieve</param>
    /// <param name="fallbackValue">Fallback value when no value is found</param>
    /// <returns>Value of the local storage if it exists, otherwise it returns <see cref="fallbackValue"/></returns>
    public async Task<string> GetStringAsync(string key, string fallbackValue)
    {
        if(!await ContainsKeyAsync(key))
            return fallbackValue;

        return await GetStringAsync(key);
    }

    /// <summary>
    /// Retrieves the object for a specific key.
    /// Uses json parsing to construct the object.
    /// Will throw an exception if not found or malformed.
    /// It's recommended to use <see cref="GetAsync{T}(string, T)"/>
    /// </summary>
    /// <param name="key">Key of the object to retrieve</param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>Object retrieved from the local storage</returns>
    public async Task<T> GetAsync<T>(string key)
        => JsonSerializer.Deserialize<T>(await GetStringAsync(key))!;
    
    /// <summary>
    /// Retrieves the object for a specific key.
    /// Uses json parsing to construct the object.
    /// Will return the default value if not found or malformed
    /// </summary>
    /// <param name="key">Key of the object to retrieve</param>
    /// <param name="defaultValue">Default value which will be used when value is not found or malformed</param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>Object retrieved from the local storage</returns>
    public async Task<T> GetAsync<T>(string key, T defaultValue)
    {
        if(!await ContainsKeyAsync(key))
            return defaultValue;

        try
        {
            return await GetAsync<T>(key);
        }
        catch (JsonException)
        {
            return defaultValue;
        }
    }
}