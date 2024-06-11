namespace MoonCore.Blazor.Models;

public class MoonCoreBlazorConfiguration
{
    public LazyLoaderData LazyLoader { get; set; } = new();
    public ToastData Toast { get; set; } = new();
    
    public class LazyLoaderData
    {
        public TimeSpan TimeUntilSpinnerIsShown { get; set; } = TimeSpan.FromSeconds(1);
    }
    
    public class ToastData
    {
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
    }
}