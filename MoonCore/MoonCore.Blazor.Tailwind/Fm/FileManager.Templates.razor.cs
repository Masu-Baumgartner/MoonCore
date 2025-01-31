using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.Fm;

public partial class FileManager
{
    [Parameter] public RenderFragment<FileSystemEntry>? ContextMenuTemplate { get; set; }
    [Parameter] public RenderFragment<FileSystemEntry[]>? SelectionActionTemplate { get; set; }
    [Parameter] public RenderFragment<FileSystemEntry>? EntryTemplate { get; set; }
    [Parameter] public RenderFragment? ActionTemplate { get; set; }
}