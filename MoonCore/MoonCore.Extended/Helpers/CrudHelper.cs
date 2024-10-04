using Microsoft.Extensions.DependencyInjection;
using MoonCore.Exceptions;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Configuration;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Extended.Helpers;

public class CrudHelper<T> where T : class
{
    private readonly DatabaseRepository<T> Repository;
    private readonly IServiceProvider ServiceProvider;

    public CrudHelper(DatabaseRepository<T> repository, IServiceProvider serviceProvider)
    {
        Repository = repository;
        ServiceProvider = serviceProvider;
    }

    public Task<IPagedData<T>> Get(int page, int pageSize)
    {
        var configuration = GetConfig();

        if (pageSize < configuration.MaxItemLimit)
            throw new HttpApiException("The requested page size is too large", 400);

        var totalItems = Repository.Get().Count();
        
        var items = Repository
            .Get()
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult<IPagedData<T>>(new PagedData<T>()
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = pageSize == 0 ? 0 : totalItems / pageSize
        });
    }

    public Task<T> GetSingle(int id)
    {
        var item = Repository
            .Get()
            .Find(id);

        if (item == null)
            throw new HttpApiException("No item with this id found", 404);

        return Task.FromResult(item);
    }

    public Task<T> Create(object data)
    {
        var item = Mapper.Map<T>(data);

        var finalItem = Repository.Add(item);

        return Task.FromResult(finalItem);
    }

    public async Task<T> Update(int id, object data)
    {
        var item = await GetSingle(id);
        
        item = Mapper.Map(item, data);
        
        Repository.Update(item);

        return item;
    }

    public async Task Delete(int id)
    {
        var item = await GetSingle(id);
        
        Repository.Delete(item);
    }

    private CrudHelperConfiguration GetConfig() =>
        ServiceProvider.GetService<CrudHelperConfiguration>() ?? new();
}