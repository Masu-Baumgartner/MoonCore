using Microsoft.Extensions.DependencyInjection;
using MoonCore.Exceptions;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Configuration;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Extended.Helpers;

public class CrudHelper<T, TResult> where T : class
{
    private readonly DatabaseRepository<T> Repository;
    private readonly IServiceProvider ServiceProvider;

    public Func<IQueryable<T>, IQueryable<T>>? QueryModifier { get; set; }
    public Func<T, TResult, TResult>? LateMapper { get; set; }

    public CrudHelper(DatabaseRepository<T> repository, IServiceProvider serviceProvider)
    {
        Repository = repository;
        ServiceProvider = serviceProvider;
    }

    public Task<IPagedData<TResult>> Get(int page, int pageSize)
    {
        var configuration = GetConfig();

        if (pageSize > configuration.MaxItemLimit)
            throw new HttpApiException("The requested page size is too large", 400);

        var totalItems = Repository.Get().Count();

        var items = Repository
            .Get() as IQueryable<T>;

        if (QueryModifier != null)
            items = QueryModifier.Invoke(items);

        var loadedItems = items
            .Skip(page * pageSize)
            .Take(pageSize).ToArray();

        var castedItems = loadedItems
            .Select(MapToResult)
            .ToArray();

        return Task.FromResult<IPagedData<TResult>>(new PagedData<TResult>()
        {
            Items = castedItems,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = pageSize == 0 ? 0 : totalItems / pageSize
        });
    }

    public async Task<TResult> GetSingle(int id)
    {
        var item = await GetSingleModel(id);

        var castedItem = MapToResult(item);
        
        return castedItem;
    }
    
    public Task<T> GetSingleModel(int id)
    {
        var query = Repository
            .Get();

        T? item;

        if (QueryModifier == null)
            item = query.FirstOrDefaultById(id);
        else
            item = QueryModifier.Invoke(query).FirstOrDefaultById(id);

        if (item == null)
            throw new HttpApiException("No item with this id found", 404);
        
        return Task.FromResult(item);
    }

    public Task<TResult> Create(object data)
    {
        var item = Mapper.Map<T>(data);

        var finalItem = Repository.Add(item);

        var castedItem = MapToResult(finalItem);

        return Task.FromResult(castedItem);
    }

    public async Task<TResult> Update(int id, object data)
    {
        var item = await GetSingleModel(id);

        return await Update(item, data);
    }
    
    public Task<TResult> Update(T item, object data)
    {
        item = Mapper.Map(item, data, ignoreNullValues: true);
        
        Repository.Update(item);

        var castedItem = MapToResult(item);

        return Task.FromResult(castedItem);
    }

    public async Task Delete(int id)
    {
        var item = await GetSingleModel(id);
        
        Repository.Delete(item);
    }

    public TResult MapToResult(T item)
    {
        var mapped = Mapper.Map<TResult>(item);

        if (LateMapper != null)
            mapped = LateMapper.Invoke(item, mapped);

        return mapped;
    }

    private CrudHelperConfiguration GetConfig() =>
        ServiceProvider.GetService<CrudHelperConfiguration>() ?? new();
}