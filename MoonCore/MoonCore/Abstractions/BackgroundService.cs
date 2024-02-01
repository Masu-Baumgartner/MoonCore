namespace MoonCore.Abstractions;

/// <summary>
/// A abstraction of a mooncore background service
/// </summary>
public abstract class BackgroundService
{
    public CancellationTokenSource Cancellation = new();

    /// <summary>
    /// This method is started in its own task and as such can run as long as it wants
    /// </summary>
    /// <returns></returns>
    public abstract Task Run();

    public Task Stop()
    {
        Cancellation.Cancel();
        return Task.CompletedTask;
    }
}