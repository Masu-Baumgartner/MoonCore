namespace MoonCore.Helpers;

public class Debouncer
{
    private readonly TimeSpan Delay;
    private CancellationTokenSource Cts = new();

    public Debouncer(int delay = 300)
    {
        Delay = TimeSpan.FromMilliseconds(delay);
    }
    
    public Debouncer(TimeSpan delay)
    {
        Delay = delay;
    }

    public void Debounce(Func<Task> action)
    {
        // Cancel any existing scheduled task
        Cts.Cancel();
        Cts.Dispose();

        // Create a new token
        Cts = new CancellationTokenSource();
        var token = Cts.Token;

        // Schedule the task
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Delay, token);
                if (!token.IsCancellationRequested)
                {
                    await action();
                }
            }
            catch (TaskCanceledException)
            {
                // Expected if a new debounce event happens before the delay
            }
        }, token);
    }

    public void Debounce(Action action)
    {
        Debounce(() =>
        {
            action();
            return Task.CompletedTask;
        });
    }
}