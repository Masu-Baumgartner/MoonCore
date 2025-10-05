namespace MoonCore.Blazor.FlyonUi.Common;

/// <summary>
/// Factory to create item sources from methods, lambda expressions and delegates
/// </summary>
public static class ItemSourceFactory
{
    #region Basic Sources

    /// <summary>
    /// Creates an item source from a static collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to serve as the data source.</param>
    /// <returns>An item source that provides the specified items.</returns>
    public static ItemSource<T> From<T>(IEnumerable<T> items)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, _, _) => ValueTask.FromResult(items),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates an item source from a synchronous callback that returns items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that returns the collection of items.</param>
    /// <returns>An item source that invokes the callback to retrieve items.</returns>
    public static ItemSource<T> From<T>(Func<IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, _, _) => ValueTask.FromResult(callback.Invoke()),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates an item source from an asynchronous callback that returns items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that returns the collection of items.</param>
    /// <returns>An item source that invokes the async callback to retrieve items.</returns>
    public static ItemSource<T> From<T>(Func<Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (_, _, _, _) => await callback.Invoke(),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates an item source from a ValueTask-based callback that returns items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that returns the collection of items.</param>
    /// <returns>An item source that invokes the ValueTask callback to retrieve items.</returns>
    public static ItemSource<T> From<T>(Func<ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, _, _) => callback.Invoke(),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = false
        };
    }

    #endregion

    #region Filterable Sources

    /// <summary>
    /// Creates a filterable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts a filter string and returns filtered items.</param>
    /// <returns>A filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, filter, _) => ValueTask.FromResult(callback.Invoke(filter)),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a filterable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts a filter string and returns filtered items.</param>
    /// <returns>A filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (_, _, filter, _) => await callback.Invoke(filter),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a filterable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts a filter string and returns filtered items.</param>
    /// <returns>A filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, filter, _) => callback.Invoke(filter),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = false
        };
    }

    #endregion

    #region Sortable Sources

    /// <summary>
    /// Creates a sortable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts a sort option and returns sorted items.</param>
    /// <returns>A sortable item source.</returns>
    public static ItemSource<T> From<T>(Func<SortOption?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, _, option) => ValueTask.FromResult(callback.Invoke(option)),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts a sort option and returns sorted items.</param>
    /// <returns>A sortable item source.</returns>
    public static ItemSource<T> From<T>(Func<SortOption?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (_, _, _, option) => await callback.Invoke(option),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts a sort option and returns sorted items.</param>
    /// <returns>A sortable item source.</returns>
    public static ItemSource<T> From<T>(Func<SortOption?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, _, option) => callback.Invoke(option),
            IsFilterable = false,
            IsPageable = false,
            IsSortable = true
        };
    }

    #endregion

    #region Pageable Sources

    /// <summary>
    /// Creates a pageable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts start index and count, and returns a page of items.</param>
    /// <returns>A pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, _, _) => ValueTask.FromResult(callback.Invoke(start, count)),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a pageable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts start index and count, and returns a page of items.</param>
    /// <returns>A pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (start, count, _, _) => await callback.Invoke(start, count),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a pageable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts start index and count, and returns a page of items.</param>
    /// <returns>A pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, _, _) => callback.Invoke(start, count),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = false
        };
    }

    #endregion

    #region Sortable + Filterable Sources

    /// <summary>
    /// Creates a sortable and filterable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts a filter string and sort option, and returns filtered and sorted items.</param>
    /// <returns>A sortable and filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, SortOption?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, filter, option) => ValueTask.FromResult(callback.Invoke(filter, option)),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable and filterable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts a filter string and sort option, and returns filtered and sorted items.</param>
    /// <returns>A sortable and filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, SortOption?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (_, _, filter, option) => await callback.Invoke(filter, option),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable and filterable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts a filter string and sort option, and returns filtered and sorted items.</param>
    /// <returns>A sortable and filterable item source.</returns>
    public static ItemSource<T> From<T>(Func<string?, SortOption?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (_, _, filter, option) => callback.Invoke(filter, option),
            IsFilterable = true,
            IsPageable = false,
            IsSortable = true
        };
    }

    #endregion

    #region Sortable + Pageable Sources

    /// <summary>
    /// Creates a sortable and pageable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts start index, count, and sort option, and returns a sorted page of items.</param>
    /// <returns>A sortable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, SortOption?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, _, option) => ValueTask.FromResult(callback.Invoke(start, count, option)),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable and pageable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts start index, count, and sort option, and returns a sorted page of items.</param>
    /// <returns>A sortable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, SortOption?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (start, count, _, option) => await callback.Invoke(start, count, option),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable and pageable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts start index, count, and sort option, and returns a sorted page of items.</param>
    /// <returns>A sortable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, SortOption?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, _, option) => callback.Invoke(start, count, option),
            IsFilterable = false,
            IsPageable = true,
            IsSortable = true
        };
    }

    #endregion

    #region Filterable + Pageable Sources

    /// <summary>
    /// Creates a filterable and pageable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts start index, count, and filter string, and returns a filtered page of items.</param>
    /// <returns>A filterable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, filter, _) => ValueTask.FromResult(callback.Invoke(start, count, filter)),
            IsFilterable = true,
            IsPageable = true,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a filterable and pageable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts start index, count, and filter string, and returns a filtered page of items.</param>
    /// <returns>A filterable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (start, count, filter, _) => await callback.Invoke(start, count, filter),
            IsFilterable = true,
            IsPageable = true,
            IsSortable = false
        };
    }

    /// <summary>
    /// Creates a filterable and pageable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts start index, count, and filter string, and returns a filtered page of items.</param>
    /// <returns>A filterable and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, filter, _) => callback.Invoke(start, count, filter),
            IsFilterable = true,
            IsPageable = true,
            IsSortable = false
        };
    }

    #endregion

    #region Sortable + Filterable + Pageable Sources

    /// <summary>
    /// Creates a sortable, filterable, and pageable item source from a synchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A function that accepts start index, count, filter string, and sort option, and returns a filtered, sorted page of items.</param>
    /// <returns>A sortable, filterable, and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, SortOption?, IEnumerable<T>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = (start, count, filter, option) =>
                ValueTask.FromResult(callback.Invoke(start, count, filter, option)),
            IsFilterable = true,
            IsPageable = true,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable, filterable, and pageable item source from an asynchronous callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">An asynchronous function that accepts start index, count, filter string, and sort option, and returns a filtered, sorted page of items.</param>
    /// <returns>A sortable, filterable, and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, SortOption?, Task<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = async (start, count, filter, option) => await callback.Invoke(start, count, filter, option),
            IsFilterable = true,
            IsPageable = true,
            IsSortable = true
        };
    }

    /// <summary>
    /// Creates a sortable, filterable, and pageable item source from a ValueTask-based callback.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="callback">A ValueTask function that accepts start index, count, filter string, and sort option, and returns a filtered, sorted page of items.</param>
    /// <returns>A sortable, filterable, and pageable item source.</returns>
    public static ItemSource<T> From<T>(Func<int, int, string?, SortOption?, ValueTask<IEnumerable<T>>> callback)
    {
        return new ItemSource<T>()
        {
            InnerCallback = callback.Invoke,
            IsFilterable = true,
            IsPageable = true,
            IsSortable = true
        };
    }

    #endregion
}