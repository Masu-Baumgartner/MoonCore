using MoonCore.Attributes;

namespace MoonCore.Blazor.Test.Services;

[HostedService]
public class HostedServiceThing : IHostedService
{
    public void DoSmth()
    {
        Console.WriteLine(":) " + GetHashCode());
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Start " + GetHashCode());
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop " + GetHashCode());
        
        return Task.CompletedTask;
    }
}