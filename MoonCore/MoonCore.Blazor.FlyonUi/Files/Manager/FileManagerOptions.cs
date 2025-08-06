using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public class FileManagerOptions
{
    public List<IFileOpenOperation> OpenOperations { get; } = new();
    public List<ISingleFsOperation> SingleFsOperations { get; } = new();
    public List<IMultiFsOperation> MultiFsOperations { get; } = new();
    public List<IToolbarOperation> ToolbarOperations { get; } = new();

    private readonly IServiceProvider ServiceProvider;

    public FileManagerOptions(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void AddOpenOperation<T>() where T : IFileOpenOperation
        => OpenOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddSingleOperation<T>() where T : ISingleFsOperation
        => SingleFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddMultiOperation<T>() where T : IMultiFsOperation
        => MultiFsOperations.Add(ServiceProvider.GetRequiredService<T>());
    
    public void AddToolbarOperation<T>() where T : IToolbarOperation
        => ToolbarOperations.Add(ServiceProvider.GetRequiredService<T>());
}