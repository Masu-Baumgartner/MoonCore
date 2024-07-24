using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public static class LoggingConstants
{
    public static Dictionary<LogLevel, (byte, byte, byte)> ColorMappings = new()
    {
        {
            LogLevel.Critical,
            (255, 0, 0)
        },
        {
            LogLevel.Error,
            (255, 0, 0)
        },
        {
            LogLevel.Warning,
            (215, 215, 0)
        },
        {
            LogLevel.Information,
            (135, 215, 255)
        },
        {
            LogLevel.Debug,
            (198, 198, 198)
        },
        {
            LogLevel.Trace,
            (68, 68, 68)
        }
    };

    public static Dictionary<LogLevel, string> ShortTextMappings = new()
    {
        {
            LogLevel.Critical,
            "CRIT"
        },
        {
            LogLevel.Error,
            "ERRO"
        },
        {
            LogLevel.Warning,
            "WARN"
        },
        {
            LogLevel.Information,
            "INFO"
        },
        {
            LogLevel.Debug,
            "DEBG"
        },
        {
            LogLevel.Trace,
            "TRCE"
        }
    };
}