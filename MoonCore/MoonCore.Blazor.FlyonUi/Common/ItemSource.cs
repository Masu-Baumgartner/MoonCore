namespace MoonCore.Blazor.FlyonUi.Common;

/// <summary>
/// Represents a flexible data source for items with support for sorting, pagination, and filtering.
/// </summary>
/// <typeparam name="T">The type of items in the source.</typeparam>
public struct ItemSource<T>
{
    /// <summary>
    /// Gets a value indicating whether this item source supports sorting operations.
    /// </summary>
    public bool IsSortable { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this item source supports pagination operations.
    /// </summary>
    public bool IsPageable { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this item source supports filtering operations.
    /// </summary>
    public bool IsFilterable { get; internal set; }

    internal Func<int, int, string?, SortOption?, ValueTask<IEnumerable<T>>> InnerCallback;

    /// <summary>
    /// Queries the item source asynchronously with the specified parameters.
    /// </summary>
    /// <param name="startIndex">The zero-based starting index for pagination.</param>
    /// <param name="count">The maximum number of items to retrieve.</param>
    /// <param name="filter">An optional filter string to apply to the items.</param>
    /// <param name="sortOption">An optional sort option to apply to the items.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the queried items.</returns>
    public async ValueTask<IEnumerable<T>> QueryAsync(int startIndex, int count, string? filter, SortOption? sortOption)
    {
        return await InnerCallback.Invoke(startIndex, count, filter, sortOption);
    }
}