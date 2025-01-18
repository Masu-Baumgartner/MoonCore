using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Test.UI.Ace;

public class CodeEditorService
{
    private readonly IJSRuntime JsRuntime;

    public CodeEditorService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Attach(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCoreCodeEditor.attach", id, options);
    }

    public async Task UpdateOptions(string id, CodeEditorOptions options)
    {
        await JsRuntime.InvokeVoidAsync("moonCoreCodeEditor.updateOptions", id, options);
    }

    public async Task<string> GetValue(string id)
    {
        return await JsRuntime.InvokeAsync<string>("moonCoreCodeEditor.getValue", id);
    }

    public async Task Destroy(string id)
    {
        await JsRuntime.InvokeVoidAsync("moonCoreCodeEditor.destroy", id);
    }
}