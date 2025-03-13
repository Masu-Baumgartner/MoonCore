using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Test.Xhr;

public class XmlHttpRequest : IAsyncDisposable
{
    public Func<XmlHttpRequestEvent, Task> OnProgress { get; set; }
    public Func<XmlHttpRequestEvent, Task> OnLoaded { get; set; }

    private IJSObjectReference ClientObject;
    private IJSRuntime JsRuntime;
    private int TrackingId;

    public void Initialize(IJSObjectReference clientObject, IJSRuntime jsRuntime)
    {
        ClientObject = clientObject;
        JsRuntime = jsRuntime;
        TrackingId = GetHashCode();
    }

    public async Task<IJSStreamReference> GetResponseStream() =>
        await JsRuntime.InvokeAsync<IJSStreamReference>("moonCoreXmlHttpRequest.getResponseStream", TrackingId);

    public async Task<string> GetResponseText() =>
        await JsRuntime.InvokeAsync<string>("moonCoreXmlHttpRequest.getProperty", TrackingId, "responseText");

    public async Task<string> GetResponseUrl() =>
        await JsRuntime.InvokeAsync<string>("moonCoreXmlHttpRequest.getProperty", TrackingId, "responseURL");

    public async Task<int> GetStatus() =>
        await JsRuntime.InvokeAsync<int>("moonCoreXmlHttpRequest.getProperty", TrackingId, "status");

    public async Task SetTimeout(int timeout) =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.setProperty", TrackingId, "timeout", timeout);

    public async Task SetResponseType(string responseType) =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.setProperty", TrackingId, "responseType", responseType);

    public async Task Abort() =>
        await ClientObject.InvokeVoidAsync("abort");

    public async Task<string> GetAllResponseHeaders() =>
        await ClientObject.InvokeAsync<string>("getAllResponseHeaders");

    public async Task<string?> GetResponseHeader(string header) =>
        await ClientObject.InvokeAsync<string?>("getResponseHeader");

    public async Task Open(string method, string url, bool async = true) =>
        await ClientObject.InvokeVoidAsync("open", method, url, async);

    public async Task OverrideMimeType(string mimeType) =>
        await ClientObject.InvokeVoidAsync("overrideMimeType", mimeType);

    public async Task SetRequestHeader(string header, string value) =>
        await ClientObject.InvokeVoidAsync("setRequestHeader", header, value);

    public async ValueTask DisposeAsync() =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.dispose", GetHashCode());

    [JSInvokable]
    public async Task TriggerProgressEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnProgress != null)
            await OnProgress.Invoke(requestEvent);
    }
    
    [JSInvokable]
    public async Task TriggerLoadedEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnLoaded != null)
            await OnLoaded.Invoke(requestEvent);
    }
}