using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Test.Xhr;

public class XmlHttpClient
{
    private readonly IJSRuntime JsRuntime;

    public async Task<XmlHttpRequest> Create()
    {
        var request = new XmlHttpRequest();
        var refObj = DotNetObjectReference.Create(request);
        
        var clientObject = await JsRuntime.InvokeAsync<IJSObjectReference>("moonCoreXmlHttpRequest", request.GetHashCode(), refObj);
        
        request.Initialize(clientObject, JsRuntime);
        
        return request;
    }
}