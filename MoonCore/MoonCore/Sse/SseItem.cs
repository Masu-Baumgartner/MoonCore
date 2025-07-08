namespace MoonCore.Sse;

public class SseItem<T>
{
    public string? Event { get; set; }
    public T Data { get; set; }
    
    public SseItem()
    {
        
    }

    public SseItem(string eventName)
    {
        Event = eventName;
    }
}