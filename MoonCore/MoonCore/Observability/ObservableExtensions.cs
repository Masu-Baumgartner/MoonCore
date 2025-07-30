namespace MoonCore.Observability;

public static class ObservableExtensions
{
    public static async ValueTask<IAsyncDisposable> SubscribeEventAsync<T>(
        this IAsyncObservable<T> observable,
        Func<T, ValueTask> onNextAsync,
        Func<Exception, ValueTask>? onErrorAsync = null,
        Func<ValueTask>? onCompleteAsync = null
    )
    {
        if(onErrorAsync == null)
            onErrorAsync = e => ValueTask.FromException(e);
        
        if(onCompleteAsync == null)
            onCompleteAsync = () => ValueTask.CompletedTask;
        
        return await observable.SubscribeAsync(
            new EventObserver<T>(onNextAsync, onErrorAsync, onCompleteAsync)
        );
    }
}