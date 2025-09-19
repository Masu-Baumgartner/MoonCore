using MoonCore.Helpers;

namespace MoonCore.Events;

/// <summary>
/// Represents the source of an event which can be subscribed to
/// </summary>
public class EventSource
{
    private readonly ConcurrentList<EventSubscription> Subscriptions = new();

    /// <summary>
    /// Subscribe to the event and registers your provided callback.
    /// Returns a <see cref="IAsyncDisposable"/> for unsubscribing
    /// </summary>
    /// <param name="callback">Callback which will get invoked when the event fires</param>
    /// <returns><see cref="IAsyncDisposable"/> for unsubscribing</returns>
    public ValueTask<IAsyncDisposable> SubscribeAsync(Func<ValueTask> callback)
    {
        var subscription = new EventSubscription(Subscriptions, callback);

        Subscriptions.Add(subscription);

        return ValueTask.FromResult<IAsyncDisposable>(subscription);
    }

    /// <summary>
    /// Fires the event and invokes the callback of every current subscriber
    /// </summary>
    public async ValueTask InvokeAsync()
    {
        foreach (var subscription in Subscriptions)
            await subscription.InvokeAsync();
    }
}

/// <summary>
/// Generic version of the <see cref="EventSource"/>.
/// Allows you to transfer an object to all subscribers when fired
/// </summary>
/// <typeparam name="T">Type of the event object</typeparam>
public class EventSource<T>
{
    private readonly ConcurrentList<EventSubscription<T>> Subscriptions = new();

    /// <summary>
    /// Subscribe to the event and registers your provided callback.
    /// Returns a <see cref="IAsyncDisposable"/> for unsubscribing
    /// </summary>
    /// <param name="callback">Callback which will get invoked when the event fires</param>
    /// <returns><see cref="IAsyncDisposable"/> for unsubscribing</returns>
    public ValueTask<IAsyncDisposable> SubscribeAsync(Func<T, ValueTask> callback)
    {
        var subscription = new EventSubscription<T>(Subscriptions, callback);

        Subscriptions.Add(subscription);

        return ValueTask.FromResult<IAsyncDisposable>(subscription);
    }

    /// <summary>
    /// Fires the event and invokes the callback of every current subscriber with the provided value
    /// </summary>
    /// <param name="value">Value to invoke the subscriber callbacks with</param>
    public async ValueTask InvokeAsync(T value)
    {
        foreach (var subscription in Subscriptions)
            await subscription.InvokeAsync(value);
    }
}