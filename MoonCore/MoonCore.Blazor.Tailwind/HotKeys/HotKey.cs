namespace MoonCore.Blazor.Tailwind.HotKeys;

public class HotKey
{
    public string Key { get; set; } = "";
    public ModifierKey Modifier { get; set; }
    public event Func<Task> OnPressed;
    public int ListenerCount { get; set; } = 0;

    public async Task Trigger()
    {
        if (OnPressed != null)
            await OnPressed.Invoke();
    }
}