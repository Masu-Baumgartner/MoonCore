namespace MoonCore.Observability;

public class EventObserver<T> : IAsyncObserver<T>
{
    private readonly Func<T, ValueTask> NextAsync;
    private readonly Func<Exception, ValueTask> ErrorAsync;
    private readonly Func<ValueTask> CompleteAsync;
    
    public EventObserver(Func<T, ValueTask> nextAsync, Func<Exception, ValueTask> errorAsync, Func<ValueTask> completeAsync)
    {
        NextAsync = nextAsync;
        ErrorAsync = errorAsync;
        CompleteAsync = completeAsync;
    }
    
    public async ValueTask OnNextAsync(T value)
    => await NextAsync(value);

    public async ValueTask OnErrorAsync(Exception error)
    => await ErrorAsync(error);

    public async ValueTask OnCompletedAsync()
     =>  await CompleteAsync();
}