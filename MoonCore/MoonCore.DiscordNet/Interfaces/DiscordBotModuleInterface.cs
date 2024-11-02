namespace MoonCore.DiscordNet.Interfaces;

public interface DiscordBotModuleInterface
{
    /// <summary>
    /// This is an (async) implementation to Load EventHandlers
    /// </summary>
    public Task InitializeAsync();

    /// <summary>
    /// This is an (async) implementation to Unload EventHandlers
    /// </summary>
    public Task UnloadAsync();
    
    /// <summary>
    /// This is an (async) implementation to Registering for Example SlashCommands
    /// </summary>
    public Task RegisterAsync();
}