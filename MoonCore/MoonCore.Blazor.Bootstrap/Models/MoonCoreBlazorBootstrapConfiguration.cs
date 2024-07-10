using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Bootstrap.Models;

public class MoonCoreBlazorBootstrapConfiguration
{
    public LazyLoaderData LazyLoader { get; set; } = new();
    public ToastData Toast { get; set; } = new();
    public ErrorHandlerData ErrorHandler { get; set; } = new();
    
    public class LazyLoaderData
    {
        public TimeSpan TimeUntilSpinnerIsShown { get; set; } = TimeSpan.FromSeconds(1);
    }
    
    public class ToastData
    {
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
    }
    
    public class ErrorHandlerData
    {
        public Func<IServiceProvider, bool>? ShowStacktraceFunc { get; set; }
        public RenderFragment? UnknownErrorComponent { get; set; }
        public RenderFragment<RenderFragment>? StacktraceComponent { get; set; }
        public RenderFragment<string>? ErrorMessageComponent { get; set; }
        public TimeSpan DisplayErrorDisappearTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}