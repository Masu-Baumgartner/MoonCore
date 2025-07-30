using MoonCore.Helpers;

namespace MoonCore.Observability;

public class SubjectUnsubscriber<T> : IAsyncDisposable
{
    private readonly ConcurrentList<IAsyncObserver<T>> Subscribers;
    private readonly IAsyncObserver<T> Observer;

    public SubjectUnsubscriber(ConcurrentList<IAsyncObserver<T>> subscribers, IAsyncObserver<T> observer)
    {
        Subscribers = subscribers;
        Observer = observer;
    }

    public ValueTask DisposeAsync()
    {
        Subscribers.Remove(Observer);
        return ValueTask.CompletedTask;
    }
}