using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.FlyonUi.Ace;
using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.BrowserStorage;
using MoonCore.Blazor.FlyonUi.Drawers;
using MoonCore.Blazor.FlyonUi.Exceptions;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Operations;
using MoonCore.Blazor.FlyonUi.Helpers;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Toasts;

namespace MoonCore.Blazor.FlyonUi;

/// <summary>
/// Service collection extensions to quickly register FlyonUI services
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds all basic flyonui services like the modal service and the cookie service
    /// </summary>
    /// <param name="collection">Service collection to add the services to</param>
    public static void AddFlyonUiServices(this IServiceCollection collection)
    {
        collection.AddScoped<ModalService>();
        collection.AddScoped<AlertService>();
        collection.AddScoped<ToastService>();
        collection.AddScoped<CodeEditorService>();
        collection.AddScoped<DropHandlerService>();
        collection.AddScoped<DownloadService>();
        collection.AddScoped<GlobalErrorService>();
        collection.AddScoped<CookieService>();
        collection.AddScoped<LocalStorageService>();
        collection.AddScoped<DrawerService>();
    }

    /// <summary>
    /// Adds all file manager operations to the dependency injection
    /// </summary>
    /// <param name="collection">Service collection to add the services to</param>
    public static void AddFileManagerOperations(this IServiceCollection collection)
    {
        collection.AddScoped<DeleteOperation>();
        collection.AddScoped<MoveOperation>();
        collection.AddScoped<DownloadOperation>();
        collection.AddScoped<RenameOperation>();
        collection.AddScoped<CreateFileOperation>();
        collection.AddScoped<CreateFolderOperation>();

        collection.AddScoped<ArchiveOperation>();

        collection.AddScoped<EditorOpenOperation>();
        collection.AddScoped<ImageOpenOperation>();
        collection.AddScoped<VideoOpenOperation>();
    }
}