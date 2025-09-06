using MoonCore.Helpers;

namespace MoonCore.Events;

public class EventSubscription : IAsyncDisposable
{
    private readonly ConcurrentList<EventSubscription> Subscriptions;
    private readonly Func<ValueTask> Callback;

    public EventSubscription(ConcurrentList<EventSubscription> subscriptions, Func<ValueTask> callback)
    {
        Subscriptions = subscriptions;
        Callback = callback;
    }

    public async ValueTask InvokeAsync()
        => await Callback.Invoke();

    public ValueTask DisposeAsync()
    {
        Subscriptions.Remove(this);
        return ValueTask.CompletedTask;
    }
}

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

    public async ValueTask InvokeAsync(T value)
        => await Callback.Invoke(value);

    public ValueTask DisposeAsync()
    {
        Subscriptions.Remove(this);
        return ValueTask.CompletedTask;
    }
}