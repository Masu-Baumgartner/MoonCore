using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers;

/// <summary>
/// This is a async implementation of the event handler which works nearly the same
/// </summary>
public class SmartEventHandler
{
    private readonly ILogger<SmartEventHandler> Logger;
    
    private readonly List<Func<Task>> Subscribers = new();

    public SmartEventHandler(ILogger<SmartEventHandler> logger)
    {
        Logger = logger;
    }

    public static SmartEventHandler operator +(SmartEventHandler handler, Func<Task> callback)
    {
        lock (handler.Subscribers)
            handler.Subscribers.Add(callback);

        return handler;
    }

    public static SmartEventHandler operator -(SmartEventHandler handler, Func<Task> callback)
    {
        lock (handler.Subscribers)
            handler.Subscribers.Remove(callback);

        return handler;
    }

    /// <summary>
    /// This invokes every listener in a new task
    /// </summary>
    /// <returns></returns>
    public Task Invoke()
    {
        List<Func<Task>> callbacks;

        lock (Subscribers)
            callbacks = Subscribers.ToList();

        foreach (var callback in callbacks)
        {
            Task.Run(async () =>
            {
                try
                {
                    await callback.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogWarning("An unhandled error occured while processing an api callback: {e}", e);
                }
            });
        }

        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Clears all subscribes of the event
    /// </summary>
    /// <returns></returns>
    public Task ClearSubscribers()
    {
        lock (Subscribers)
            Subscribers.Clear();
        
        return Task.CompletedTask;
    }
}

/// <summary>
/// This is a async implementation of the event handler which works nearly the same
/// </summary>
public class SmartEventHandler<T>
{
    private readonly ILogger<SmartEventHandler> Logger;
    
    private readonly List<Func<T, Task>> Subscribers = new();

    public SmartEventHandler(ILogger<SmartEventHandler> logger)
    {
        Logger = logger;
    }

    public static SmartEventHandler<T> operator +(SmartEventHandler<T> handler, Func<T, Task> callback)
    {
        lock (handler.Subscribers)
            handler.Subscribers.Add(callback);

        return handler;
    }

    public static SmartEventHandler<T> operator -(SmartEventHandler<T> handler, Func<T, Task> callback)
    {
        lock (handler.Subscribers)
            handler.Subscribers.Remove(callback);

        return handler;
    }

    /// <summary>
    /// This invokes every listener in a new task 
    /// </summary>
    /// <param name="data">(optional) the parameter of the event handler</param>
    /// <returns></returns>
    public Task Invoke(T? data = default(T))
    {
        List<Func<T, Task>> callbacks;

        lock (Subscribers)
            callbacks = Subscribers.ToList();

        foreach (var callback in callbacks)
        {
            Task.Run(async () =>
            {
                try
                {
                    await callback.Invoke(data!);
                }
                catch (Exception e)
                {
                    Logger.LogWarning("An unhandled error occured while processing an api callback: {e}", e);
                }
            });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all subscribes of the event
    /// </summary>
    /// <returns></returns>
    public Task ClearSubscribers()
    {
        lock (Subscribers)
            Subscribers.Clear();
        
        return Task.CompletedTask;
    }
}