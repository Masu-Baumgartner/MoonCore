using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Ace;

public class CodeEditorService
{
    private readonly IJSRuntime JsRuntime;

    public CodeEditorService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task AttachAsync(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.attach", id, options);
    }

    public async Task UpdateOptionsAsync(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.updateOptions", id, options);
    }

    public async Task<string> GetValueAsync(string id)
    {
        return await JsRuntime.InvokeAsync<string>("moonCore.codeEditor.getValue", id);
    }

    public async Task DestroyAsync(string id)
    {
        await JsRuntime.InvokeVoidAsync("moonCore.codeEditor.destroy", id);
    }
}