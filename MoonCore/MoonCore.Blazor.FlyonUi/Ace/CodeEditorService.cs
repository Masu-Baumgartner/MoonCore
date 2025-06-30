using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Ace;

public class CodeEditorService
{
    private readonly IJSRuntime JsRuntime;

    public CodeEditorService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Attach(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.attach", id, options);
    }

    public async Task UpdateOptions(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.updateOptions", id, options);
    }

    public async Task<string> GetValue(string id)
    {
        return await JsRuntime.InvokeAsync<string>("moonCore.codeEditor.getValue", id);
    }

    public async Task Destroy(string id)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.destroy", id);
    }
}