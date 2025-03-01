using System.Collections.Frozen;
using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public static class LoggingConstants
{
    public static readonly FrozenDictionary<LogLevel, (byte, byte, byte)> ColorMappings = new Dictionary<LogLevel, (byte, byte, byte)>()
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
    }.ToFrozenDictionary();

    public static FrozenDictionary<LogLevel, string> ShortTextMappings = new Dictionary<LogLevel, string>()
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
    }.ToFrozenDictionary();
}