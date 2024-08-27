using System.Net.Http.Headers;
using System.Text.Json;
using MoonCore.Extensions;

namespace MoonCore.Helpers;

public class HttpApiClient
{
    private readonly HttpClient Client;

    public HttpApiClient(HttpClient client)
    {
        Client = client;
    }

    #region GET

    public Task<HttpResponseMessage> Get(string url) => SendHandled(HttpMethod.Get, url);
    
    public async Task<Stream> GetStream(string url)
    {
        var response = await SendHandled(HttpMethod.Get, url);
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> GetString(string url)
    {
        var response = await SendHandled(HttpMethod.Get, url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> GetJson<T>(string url)
    {
        var json = await GetString(url);
        return JsonSerializer.Deserialize<T>(json) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region POST

    public Task<HttpResponseMessage> Post(string url, object? data = null) => SendHandled(HttpMethod.Post, url, ConvertToContent(data));
    
    public async Task<Stream> PostStream(string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Post, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PostString(string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Post, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PostJson<T>(string url, object? data = null)
    {
        var json = await PostString(url, data);
        return JsonSerializer.Deserialize<T>(json) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region PATCH

    public Task<HttpResponseMessage> Patch(string url, object? data = null) => SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
    
    public async Task<Stream> PatchStream(string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PatchString(string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PatchJson<T>(string url, object? data = null)
    {
        var json = await PatchString(url, data);
        return JsonSerializer.Deserialize<T>(json) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region PUT

    public Task<HttpResponseMessage> Put(string url, object? data = null) => SendHandled(HttpMethod.Put, url, ConvertToContent(data));
    
    public async Task<Stream> PutStream(string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Put, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PutString(string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Put, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PutJson<T>(string url, object? data = null)
    {
        var json = await PutString(url, data);
        return JsonSerializer.Deserialize<T>(json) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region DELETE

    public Task<HttpResponseMessage> Delete(string url) => SendHandled(HttpMethod.Delete, url);
    
    public async Task<Stream> DeleteStream(string url)
    {
        var response = await SendHandled(HttpMethod.Delete, url);
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> DeleteString(string url)
    {
        var response = await SendHandled(HttpMethod.Delete, url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> DeleteJson<T>(string url)
    {
        var json = await DeleteString(url);
        return JsonSerializer.Deserialize<T>(json) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    private HttpContent? ConvertToContent(object? data)
    {
        if (data == null)
            return null;

        if (data is byte[] dataBytes)
            return new ByteArrayContent(dataBytes);
        
        if (data is Stream stream)
            return new StreamContent(stream);

        if (data is string dataString)
            return new StringContent(dataString);

        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, new MediaTypeHeaderValue("application/json"));
    }
    
    private async Task<HttpResponseMessage> SendHandled(HttpMethod method, string url, HttpContent? body = null)
    {
        var request = new HttpRequestMessage(method, url);

        if (body != null)
            request.Content = body;

        var result = await Client.SendAsync(request);

        await result.HandlePossibleApiError();

        return result;
    }
}