using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public class FileManagerOptions
{
    /// <summary>
    /// All registered open operations
    /// </summary>
    public List<IFsOpenOperation> OpenOperations { get; } = new();
    
    /// <summary>
    /// All registered single operations
    /// </summary>
    public List<ISingleFsOperation> SingleFsOperations { get; } = new();
    
    /// <summary>
    /// All registered multi operations
    /// </summary>
    public List<IMultiFsOperation> MultiFsOperations { get; } = new();
    
    /// <summary>
    /// All registered toolbar operations
    /// </summary>
    public List<IToolbarOperation> ToolbarOperations { get; } = new();

    /// <summary>
    /// Maximum write limit. Used to determine the chunk size for chunked uploads and limit the uploads in general if no chunked upload is possible
    /// </summary>
    public long WriteLimit { get; set; } = ByteConverter.FromMegaBytes(20).Bytes;
    
    /// <summary>
    /// Maximum file size to allow a user to open in an <see cref="IFsOpenOperation"/>
    /// </summary>
    public long OpenLimit { get; set; } = -1;
    
    /// <summary>
    /// Hard limit for file uploads. Also affects chunked file uploads unlike <see cref="WriteLimit"/>
    /// </summary>
    public long UploadLimit { get; set; } = ByteConverter.FromGigaBytes(4).Bytes;

    private readonly IServiceProvider ServiceProvider;

    public FileManagerOptions(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Resolve an <see cref="IFsOpenOperation"/> from the dependency injection and register it to the current instance
    /// </summary>
    /// <typeparam name="T">Implementation type of <see cref="IFsOpenOperation"/></typeparam>
    public void AddOpenOperation<T>() where T : IFsOpenOperation
        => OpenOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    /// <summary>
    /// Resolve an <see cref="ISingleFsOperation"/> from the dependency injection and register it to the current instance
    /// </summary>
    /// <typeparam name="T">Implementation type of <see cref="ISingleFsOperation"/></typeparam>
    public void AddSingleOperation<T>() where T : ISingleFsOperation
        => SingleFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    /// <summary>
    /// Resolve an <see cref="IMultiFsOperation"/> from the dependency injection and register it to the current instance
    /// </summary>
    /// <typeparam name="T">Implementation type of <see cref="IMultiFsOperation"/></typeparam>
    public void AddMultiOperation<T>() where T : IMultiFsOperation
        => MultiFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    /// <summary>
    /// Resolve an <see cref="IToolbarOperation"/> from the dependency injection and register it to the current instance
    /// </summary>
    /// <typeparam name="T">Implementation type of <see cref="IToolbarOperation"/></typeparam>
    public void AddToolbarOperation<T>() where T : IToolbarOperation
        => ToolbarOperations.Add(ServiceProvider.GetRequiredService<T>());
}