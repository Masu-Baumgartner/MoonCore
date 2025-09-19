using MoonCore.Helpers;

namespace MoonCore.Events;

/// <summary>
/// Represents an active subscription to an event source
/// </summary>
public class EventSubscription : IAsyncDisposable
{
    private readonly ConcurrentList<EventSubscription> Subscriptions;
    private readonly Func<ValueTask> Callback;

    public EventSubscription(ConcurrentList<EventSubscription> subscriptions, Func<ValueTask> callback)
    {
        Subscriptions = subscriptions;
        Callback = callback;
    }

    /// <summary>
    /// Invokes the callback of the subscription
    /// </summary>
    public async ValueTask InvokeAsync()
        => await Callback.Invoke();

    /// <summary>
    /// Removes the subscription for the source. No callbacks will be received anymore
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        Subscriptions.Remove(this);
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Generic version of the <see cref="EventSubscription"/>.
/// Represents an active subscription to an event source
/// </summary>
/// <typeparam name="T">Type which the event source provides</typeparam>
public class EventSubscription<T> : IAsyncDisposable
{
    private readonly ConcurrentList<EventSubscription<T>> Subscriptions;
    private readonly Func<T, ValueTask> Callback;

    public EventSubscription(
        ConcurrentList<EventSubscription<T>> subscriptions,
        Func<T, ValueTask> callback
    )
    {
        Subscriptions = subscriptions;
        Callback = callback;
    }

    /// <summary>
    /// Invokes the callback of the subscription with the provided value
    /// </summary>
    /// <param name="value">Value to pass to the callback</param>
    public async ValueTask InvokeAsync(T value)
        => await Callback.Invoke(value);

    /// <summary>
    /// Removes the subscription for the source. No callbacks will be received anymore
    /// </summary>
    /// <returns></returns>
    
    public ValueTask DisposeAsync()
    {
        Subscriptions.Remove(this);
        return ValueTask.CompletedTask;
    }
}