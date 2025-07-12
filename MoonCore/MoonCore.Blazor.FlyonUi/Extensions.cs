using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.FlyonUi.Ace;
using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Operations;
using MoonCore.Blazor.FlyonUi.Helpers;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;

namespace MoonCore.Blazor.FlyonUi;

public static class Extensions
{
    public static void AddFlyonUiServices(this IServiceCollection collection)
    {
        collection.AddScoped<ModalService>();
        collection.AddScoped<AlertService>();
        collection.AddScoped<ToastService>();
        collection.AddScoped<CodeEditorService>();
        collection.AddScoped<ChunkedFileTransferService>();
        collection.AddScoped<DropHandlerService>();
        collection.AddScoped<DownloadService>();
    }

    public static void AddFileManagerOperations(this IServiceCollection collection)
    {
        collection.AddScoped<IMultiFsOperation, DeleteOperation>();
        collection.AddScoped<IMultiFsOperation, MoveOperation>();
        collection.AddScoped<IMultiFsOperation, DownloadOperation>();
        collection.AddScoped<ISingleFsOperation, RenameOperation>();
        collection.AddScoped<IToolbarOperation, CreateFileOperation>();
        collection.AddScoped<IToolbarOperation, CreateFolderOperation>();
    }
}