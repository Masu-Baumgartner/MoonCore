using Microsoft.EntityFrameworkCore;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Extended.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedData<T>> ToPagedData<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var count = await query.CountAsync();
        
        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize).ToArrayAsync();
        
        return new PagedData<T>()
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = count,
            TotalPages = pageSize == 0 ? 0 : count / pageSize
        };
    }
    
    public static async Task<PagedData<TResult>> ToPagedDataMapped<T, TResult>(this IQueryable<T> query, int page, int pageSize)
    {
        var count = await query.CountAsync();
        
        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        var castedItems = items
            .Select(x => Mapper.Map<TResult>(x!))
            .ToArray();
        
        return new PagedData<TResult>()
        {
            Items = castedItems,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = count,
            TotalPages = pageSize == 0 ? 0 : count / pageSize
        };
    }
}