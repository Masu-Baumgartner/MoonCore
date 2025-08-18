using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using MoonCore.Extensions;

namespace MoonCore.Helpers;

public class HttpApiClient : IDisposable
{
    public event Func<HttpRequestMessage, Task>? OnConfigureRequest;
    public bool PreventDisposeClient { get; set; } = false;
    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly HttpClient Client;

    public HttpApiClient(HttpClient client)
    {
        Client = client;
    }

    public HttpClient GetBaseClient() => Client;

    #region GET

    public Task<HttpResponseMessage> Get([StringSyntax(StringSyntaxAttribute.Uri)] string url) => SendHandled(HttpMethod.Get, url);
    
    public async Task<Stream> GetStream([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var response = await SendHandled(HttpMethod.Get, url);
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> GetString([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var response = await SendHandled(HttpMethod.Get, url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> GetJson<T>([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var json = await GetString(url);
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region POST

    public Task<HttpResponseMessage> Post([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null) => SendHandled(HttpMethod.Post, url, ConvertToContent(data));
    
    public async Task<Stream> PostStream([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Post, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PostString([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Post, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PostJson<T>([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var json = await PostString(url, data);
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region PATCH

    public Task<HttpResponseMessage> Patch([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null) => SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
    
    public async Task<Stream> PatchStream([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PatchString([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Patch, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PatchJson<T>([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var json = await PatchString(url, data);
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region PUT

    public Task<HttpResponseMessage> Put([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null) => SendHandled(HttpMethod.Put, url, ConvertToContent(data));
    
    public async Task<Stream> PutStream([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response = await SendHandled(HttpMethod.Put, url, ConvertToContent(data));
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> PutString([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var response =await SendHandled(HttpMethod.Put, url, ConvertToContent(data));
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> PutJson<T>([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? data = null)
    {
        var json = await PutString(url, data);
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    #region DELETE

    public Task<HttpResponseMessage> Delete([StringSyntax(StringSyntaxAttribute.Uri)] string url) => SendHandled(HttpMethod.Delete, url);
    
    public async Task<Stream> DeleteStream([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var response = await SendHandled(HttpMethod.Delete, url);
        return await response.Content.ReadAsStreamAsync();
    }
    
    public async Task<string> DeleteString([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var response = await SendHandled(HttpMethod.Delete, url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<T> DeleteJson<T>([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var json = await DeleteString(url);
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new JsonException($"Unable to parse json: {json}");
    }

    #endregion

    private HttpContent? ConvertToContent(object? data)
    {
        if (data == null)
            return null;
        
        if(data.GetType().IsSubclassOf(typeof(HttpContent)))
            return data as HttpContent;

        if (data is byte[] dataBytes)
            return new ByteArrayContent(dataBytes);
        
        if (data is Stream stream)
            return new StreamContent(stream);

        if (data is string dataString)
            return new StringContent(dataString);

        var json = JsonSerializer.Serialize(data, JsonSerializerOptions);
        return new StringContent(json, new MediaTypeHeaderValue("application/json"));
    }
    
    private async Task<HttpResponseMessage> SendHandled(HttpMethod method, string url, HttpContent? body = null)
    {
        var request = new HttpRequestMessage(method, url);

        if (body != null)
            request.Content = body;

        if(OnConfigureRequest != null)
            await OnConfigureRequest.Invoke(request);

        var result = await Client.SendAsync(request);

        await result.HandlePossibleApiError();

        return result;
    }

    public void Dispose()
    {
        if(PreventDisposeClient)
            return;
        
        Client.Dispose();
    }
}