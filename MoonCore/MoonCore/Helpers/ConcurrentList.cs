using System.Collections;

namespace MoonCore.Helpers;

/// <summary>
/// Threadsafe implementation of <see cref="IList{T}"/>
/// </summary>
/// <typeparam name="T">Type to store in the list</typeparam>
public class ConcurrentList<T> : IList<T>
{
    private readonly List<T> InnerList = new List<T>();
    private readonly ReaderWriterLockSlim ock = new ReaderWriterLockSlim();

    /// <inheritdoc />
    public T this[int index]
    {
        get
        {
            ock.EnterReadLock();
            try { return InnerList[index]; }
            finally { ock.ExitReadLock(); }
        }
        set
        {
            ock.EnterWriteLock();
            try { InnerList[index] = value; }
            finally { ock.ExitWriteLock(); }
        }
    }

    /// <inheritdoc />
    public int Count
    {
        get
        {
            ock.EnterReadLock();
            try { return InnerList.Count; }
            finally { ock.ExitReadLock(); }
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item)
    {
        ock.EnterWriteLock();
        try { InnerList.Add(item); }
        finally { ock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public void Clear()
    {
        ock.EnterWriteLock();
        try { InnerList.Clear(); }
        finally { ock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        ock.EnterReadLock();
        try { return InnerList.Contains(item); }
        finally { ock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        ock.EnterReadLock();
        try { InnerList.CopyTo(array, arrayIndex); }
        finally { ock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;
        ock.EnterReadLock();
        try { snapshot = new List<T>(InnerList); }
        finally { ock.ExitReadLock(); }

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        ock.EnterReadLock();
        try { return InnerList.IndexOf(item); }
        finally { ock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        ock.EnterWriteLock();
        try { InnerList.Insert(index, item); }
        finally { ock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        ock.EnterWriteLock();
        try { return InnerList.Remove(item); }
        finally { ock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        ock.EnterWriteLock();
        try { InnerList.RemoveAt(index); }
        finally { ock.ExitWriteLock(); }
    }

    /// <summary>
    /// Removes a range of elements from the list
    /// </summary>
    /// <param name="index">Index to start deleting elements</param>
    /// <param name="count">Amount of elements to remove</param>
    public void RemoveRange(int index, int count)
    {
        ock.EnterWriteLock();
        try { InnerList.RemoveRange(index, count); }
        finally { ock.ExitWriteLock(); }
    }
}