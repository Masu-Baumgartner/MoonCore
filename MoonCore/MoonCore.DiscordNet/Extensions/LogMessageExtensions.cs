using Discord;
using Microsoft.Extensions.Logging;

namespace MoonCore.DiscordNet.Extensions;

public static class LogMessageExtensions
{
    public static void ToILogger(this LogMessage message, ILogger logger)
    {
        switch (message.Severity)
        {
            case LogSeverity.Verbose:
                logger.LogTrace(message.Message);
            
                if(message.Exception is not null)
                    logger.LogTrace("Exception: {e}", message.Exception);
            
                break;
        
            case LogSeverity.Debug:
                logger.LogDebug(message.Message);
            
                if(message.Exception is not null)
                    logger.LogDebug("Exception: {e}", message.Exception);
            
                break;
        
            case LogSeverity.Info:
                logger.LogInformation(message.Message);
            
                if(message.Exception is not null)
                    logger.LogInformation("Exception: {e}", message.Exception);
            
                break;
        
            case LogSeverity.Warning:
                logger.LogWarning(message.Message);
            
                if(message.Exception is not null)
                    logger.LogWarning("Exception: {e}", message.Exception);
            
                break;
        
            case LogSeverity.Error:
                logger.LogError(message.Message);
            
                if(message.Exception is not null)
                    logger.LogError("Exception: {e}", message.Exception);
            
                break;
        
            case LogSeverity.Critical:
                logger.LogCritical(message.Message);
            
                if(message.Exception is not null)
                    logger.LogCritical("Exception: {e}", message.Exception);
            
                break;
        }
    }
}