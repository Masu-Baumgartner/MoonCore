using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public class FileManagerOptions
{
    public List<IFsOpenOperation> OpenOperations { get; } = new();
    public List<ISingleFsOperation> SingleFsOperations { get; } = new();
    public List<IMultiFsOperation> MultiFsOperations { get; } = new();
    public List<IToolbarOperation> ToolbarOperations { get; } = new();

    public long WriteLimit { get; set; } = ByteConverter.FromMegaBytes(20).Bytes;
    
    public long OpenLimit { get; set; } = -1;
    public long UploadLimit { get; set; } = ByteConverter.FromGigaBytes(4).Bytes;

    private readonly IServiceProvider ServiceProvider;

    public FileManagerOptions(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void AddOpenOperation<T>() where T : IFsOpenOperation
        => OpenOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddSingleOperation<T>() where T : ISingleFsOperation
        => SingleFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddMultiOperation<T>() where T : IMultiFsOperation
        => MultiFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddToolbarOperation<T>() where T : IToolbarOperation
        => ToolbarOperations.Add(ServiceProvider.GetRequiredService<T>());
}