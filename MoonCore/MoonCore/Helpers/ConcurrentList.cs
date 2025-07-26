using System.Collections;

namespace MoonCore.Helpers;

public class ConcurrentList<T> : IList<T>
{
    private readonly List<T> InnerList = new List<T>();
    private readonly ReaderWriterLockSlim ock = new ReaderWriterLockSlim();

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

    public int Count
    {
        get
        {
            ock.EnterReadLock();
            try { return InnerList.Count; }
            finally { ock.ExitReadLock(); }
        }
    }

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        ock.EnterWriteLock();
        try { InnerList.Add(item); }
        finally { ock.ExitWriteLock(); }
    }

    public void Clear()
    {
        ock.EnterWriteLock();
        try { InnerList.Clear(); }
        finally { ock.ExitWriteLock(); }
    }

    public bool Contains(T item)
    {
        ock.EnterReadLock();
        try { return InnerList.Contains(item); }
        finally { ock.ExitReadLock(); }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ock.EnterReadLock();
        try { InnerList.CopyTo(array, arrayIndex); }
        finally { ock.ExitReadLock(); }
    }

    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;
        ock.EnterReadLock();
        try { snapshot = new List<T>(InnerList); }
        finally { ock.ExitReadLock(); }

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        ock.EnterReadLock();
        try { return InnerList.IndexOf(item); }
        finally { ock.ExitReadLock(); }
    }

    public void Insert(int index, T item)
    {
        ock.EnterWriteLock();
        try { InnerList.Insert(index, item); }
        finally { ock.ExitWriteLock(); }
    }

    public bool Remove(T item)
    {
        ock.EnterWriteLock();
        try { return InnerList.Remove(item); }
        finally { ock.ExitWriteLock(); }
    }

    public void RemoveAt(int index)
    {
        ock.EnterWriteLock();
        try { InnerList.RemoveAt(index); }
        finally { ock.ExitWriteLock(); }
    }

    public void RemoveRange(int index, int count)
    {
        ock.EnterWriteLock();
        try { InnerList.RemoveRange(index, count); }
        finally { ock.ExitWriteLock(); }
    }
}