namespace MoonCore.Blazor.Test;

public class DevelopmentConsoleService
{
    private readonly Dictionary<string, int> Counters = new();

    public void NotifyCounter(string counter)
    {
        lock (Counters)
        {
            if (Counters.ContainsKey(counter))
                Counters[counter] += 1;
            else
                Counters[counter] = 1;
        }
    }

    public (string, int)[] GetCounters() // Looks kinda ugly
    {
        lock (Counters)
        {
            return Counters
                .Select(x => (x.Key, x.Value))
                .ToArray();
        }
    }
}