using MoonCore.Helpers;

namespace MoonCore.Observability;

public class EventSubject<T> : IAsyncObservable<T>, IDisposable
{
    private readonly ConcurrentList<IAsyncObserver<T>> Subscribers = new();

    public ValueTask<IAsyncDisposable> SubscribeAsync(IAsyncObserver<T> observer)
    {
        if (!Subscribers.Contains(observer))
            Subscribers.Add(observer);

        return ValueTask.FromResult<IAsyncDisposable>(
            new SubjectUnsubscriber<T>(Subscribers, observer)
        );
    }

    public async ValueTask OnNextAsync(T value)
    {
        foreach (var subscriber in Subscribers)
            await subscriber.OnNextAsync(value);
    }

    public async ValueTask OnErrorAsync(Exception error)
    {
        foreach (var subscriber in Subscribers)
            await subscriber.OnErrorAsync(error);
    }

    public async ValueTask OnCompletedAsync()
    {
        foreach (var subscriber in Subscribers)
            await subscriber.OnCompletedAsync();
    }

    public void Dispose()
    {
        Subscribers.Clear();
    }
}