using System.Diagnostics;
using System.Reflection;
using Serilog;

namespace MoonCore.Helpers;

public class Logger
{
    public static void Setup(bool logInConsole = true, bool logInFile = false, string logPath = "", bool isDebug = false)
    {
        var logConfig = new LoggerConfiguration();
        var now = DateTime.UtcNow;

        logConfig = logConfig.Enrich.FromLogContext();

        if (logInConsole)
        {
            logConfig = logConfig.WriteTo.Console(
                outputTemplate:
                "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }

        if (logInFile)
        {
            logConfig = logConfig.WriteTo.File(logPath,
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }

        if (isDebug)
            logConfig = logConfig.MinimumLevel.Debug();
        else
            logConfig = logConfig.MinimumLevel.Information();

        Log.Logger = logConfig.CreateLogger();
    }
    
    #region String logger
    public static void Verbose(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Verbose("{Message}", message);
    }

    public static void Info(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Information("{Message}", message);
    }

    public static void Debug(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Debug("{Message}", message);
    }

    public static void Error(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Error("{Message}", message);
    }

    public static void Warn(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Warning("{Message}", message);
    }

    public static void Fatal(string message, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Fatal("{Message}", message);
    }

    #endregion

    #region Exception method calls

    public static void Verbose(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Verbose(exception, "");
    }

    public static void Info(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Information(exception, "");
    }

    public static void Debug(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Debug(exception, "");
    }

    public static void Error(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Error(exception, "");
    }

    public static void Warn(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Warning(exception, "");
    }

    public static void Fatal(Exception exception, string channel = "default")
    {
        Log.ForContext("SourceContext", GetNameOfCallingClass())
            .Fatal(exception, "");
    }

    #endregion

    private static string GetNameOfCallingClass(int skipFrames = 4)
    {
        try
        {
            string fullName;
            Type declaringType;

            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }

                skipFrames++;
                if (declaringType.Name.Contains("<"))
                    fullName = declaringType.ReflectedType.Name;
                else
                    fullName = declaringType.Name;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase) |
                     fullName.Contains("Logger"));

            return fullName;
        }
        catch (Exception)
        {
            return "Program*";
        }
    }
}