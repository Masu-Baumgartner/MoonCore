using MoonCore.Helpers;

namespace MoonCore.Events;

public class EventSource
{
    private readonly ConcurrentList<EventSubscription> Subscriptions = new();

    public ValueTask<IAsyncDisposable> SubscribeAsync(Func<ValueTask> callback)
    {
        var subscription = new EventSubscription(Subscriptions, callback);

        Subscriptions.Add(subscription);

        return ValueTask.FromResult<IAsyncDisposable>(subscription);
    }

    public async ValueTask InvokeAsync()
    {
        foreach (var subscription in Subscriptions)
            await subscription.InvokeAsync();
    }
}

public class EventSource<T>
{
    private readonly ConcurrentList<EventSubscription<T>> Subscriptions = new();

    public ValueTask<IAsyncDisposable> SubscribeAsync(Func<T, ValueTask> callback)
    {
        var subscription = new EventSubscription<T>(Subscriptions, callback);

        Subscriptions.Add(subscription);

        return ValueTask.FromResult<IAsyncDisposable>(subscription);
    }

    public async ValueTask InvokeAsync(T value)
    {
        foreach (var subscription in Subscriptions)
            await subscription.InvokeAsync(value);
    }
}