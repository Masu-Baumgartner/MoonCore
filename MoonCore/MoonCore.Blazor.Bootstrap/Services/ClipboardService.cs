using Microsoft.JSInterop;
using MoonCore.Blazor.Bootstrap.Exceptions;
using MoonCore.Blazor.Bootstrap.Extensions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Bootstrap.Services;

public class ClipboardService
{
    private readonly IJSRuntime JsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Copy(string content)
    {
        await JsRuntime.InvokeVoidAsyncHandled("mooncore.blazor.clipboard.copy", content);
    }

    public async Task<string[]> ReadTypes()
    {
        return await JsRuntime.InvokeAsyncHandled<string[]>("mooncore.blazor.clipboard.readTypes");
    }

    public async Task<string> ReadContentAsString(string type, int maxSizeInBytes = 1048576)
    {
        var base64 = await ReadContentAsBase64(type, maxSizeInBytes);

        return Formatter.FromBase64ToText(base64);
    }

    public async Task<byte[]> ReadContentAsBlob(string type, int maxSizeInBytes = 1048576)
    {
        var base64 = await ReadContentAsBase64(type, maxSizeInBytes);

        return Convert.FromBase64String(base64);
    }

    private async Task<string> ReadContentAsBase64(string requestedType, int maxSize)
    {
        var content = await JsRuntime.InvokeAsyncHandled<string>("mooncore.blazor.clipboard.readData", requestedType, maxSize);

        if (content == "ERRSIZE")
            throw new PayloadTooLargeException("The clipboard item payload is too large");
        
        var separatorIndex = content.IndexOf(";", StringComparison.InvariantCultureIgnoreCase);
        var base64 = content.Substring(separatorIndex + 8, content.Length - (separatorIndex + 8));

        return base64;
    }
}