using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Xhr;

public class XmlHttpRequest : IAsyncDisposable
{
    public Func<XmlHttpRequestEvent, Task> OnDownloadProgress { get; set; }
    public Func<XmlHttpRequestEvent, Task> OnUploadProgress { get; set; }
    public Func<XmlHttpRequestEvent, Task> OnLoadend { get; set; }
    public Func<XmlHttpRequestEvent, Task> OnTimeout { get; set; }
    public Func<int, Task> OnReadyStateChange { get; set; }

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

    public async Task<int> GetReadyState() =>
        await JsRuntime.InvokeAsync<int>("moonCoreXmlHttpRequest.getProperty", TrackingId, "readyState");

    public async Task SetTimeout(int timeout) =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.setProperty", TrackingId, "timeout", timeout);

    public async Task SetResponseType(string responseType) =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.setProperty", TrackingId, "responseType", responseType);

    public async Task Abort() =>
        await ClientObject.InvokeVoidAsync("abort");

    public async Task Send() =>
        await ClientObject.InvokeVoidAsync("send");

    public async Task Send(DotNetStreamReference streamReference) =>
        await JsRuntime.InvokeVoidAsync("moonCoreXmlHttpRequest.sendStream", TrackingId, streamReference);

    public async Task Send(Stream stream) =>
        await Send(new DotNetStreamReference(stream));

    public async Task SendFile(Stream fileStream, string formName, string fileName)
    {
        await JsRuntime.InvokeVoidAsync(
            "moonCoreXmlHttpRequest.sendFile",
            TrackingId,
            formName,
            fileName,
            new DotNetStreamReference(fileStream)
        );
    }

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
    public async Task TriggerDownloadProgressEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnDownloadProgress != null)
            await OnDownloadProgress.Invoke(requestEvent);
    }
    
    [JSInvokable]
    public async Task TriggerUploadProgressEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnUploadProgress != null)
            await OnUploadProgress.Invoke(requestEvent);
    }

    [JSInvokable]
    public async Task TriggerLoadedEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnLoadend != null)
            await OnLoadend.Invoke(requestEvent);
    }

    [JSInvokable]
    public async Task TriggerTimeoutEvent(XmlHttpRequestEvent requestEvent)
    {
        if (OnTimeout != null)
            await OnTimeout.Invoke(requestEvent);
    }

    [JSInvokable]
    public async Task TriggerReadyStateChangeEvent(int readyState)
    {
        if (OnReadyStateChange != null)
            await OnReadyStateChange.Invoke(readyState);
    }
}