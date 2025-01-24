using System.Text;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Tailwind.Components;
using MoonCore.Blazor.Tailwind.HotKeys;
using MoonCore.Blazor.Tailwind.Test.UI.Ace;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public partial class FileManager
{
    [Parameter] public Action<CodeEditorOptions>? OnConfigureEditor { get; set; }

    private bool IsEditing = false;
    private FileSystemEntry EditorEntry;
    private string EditorInitialContent;
    private CodeEditor FileEditor;

    private long MaxEditorOpenSize = ByteConverter.FromMegaBytes(5).Bytes;

    private async Task OpenEditor(FileSystemEntry entry)
    {
        IsEditing = true;
        EditorEntry = entry;

        // Hot keys
        await HotKeyService.Register("KeyS", ModifierKey.Control, SaveFile);
        await HotKeyService.Register("KeyQ", ModifierKey.Control, CloseEditor);
        
        //
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadEditor(LazyLoader _)
    {
        var fs = await FileSystemProvider.Read(PathBuilder.JoinPaths(CurrentPath, EditorEntry.Name));
        var sr = new StreamReader(fs, Encoding.UTF8);

        EditorInitialContent = await sr.ReadToEndAsync();

        sr.Close();
        fs.Close();
    }

    private async Task SaveFile()
    {
        var content = await FileEditor.GetValue();

        using var dataStream = new MemoryStream(
            Encoding.UTF8.GetBytes(content)
        );

        await FileSystemProvider.Create(PathBuilder.JoinPaths(CurrentPath, EditorEntry.Name), dataStream);
        await ToastService.Success("Successfully saved changes");
    }

    private async Task CloseEditor()
    {
        await FileEditor.DisposeAsync();
        IsEditing = false;

        // Hot keys
        await HotKeyService.Unregister("KeyS", ModifierKey.Control, SaveFile);
        await HotKeyService.Unregister("KeyQ", ModifierKey.Control, CloseEditor);
        
        //
        await InvokeAsync(StateHasChanged);
    }

    private void ConfigureEditor(CodeEditorOptions editorOptions)
    {
        var mode = CodeEditorModeHelper.GetModeFromFile(EditorEntry.Name);
        editorOptions.Mode = $"ace/mode/{mode}";

        OnConfigureEditor?.Invoke(editorOptions);
    }
}