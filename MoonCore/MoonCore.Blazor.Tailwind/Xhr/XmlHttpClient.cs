using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Xhr;

public class XmlHttpClient
{
    private readonly IJSRuntime JsRuntime;

    public XmlHttpClient(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task<XmlHttpRequest> Create()
    {
        var request = new XmlHttpRequest();
        var refObj = DotNetObjectReference.Create(request);
        
        var clientObject = await JsRuntime.InvokeAsync<IJSObjectReference>("moonCoreXmlHttpRequest.initialize", request.GetHashCode(), refObj);
        
        request.Initialize(clientObject, JsRuntime);
        
        return request;
    }
}