namespace MoonCore.Models;

public class MoonCoreLoggingConfiguration
{
    public ConsoleData Console { get; set; } = new();
    public FileLoggingData FileLogging { get; set; } = new();

    public class ConsoleData
    {
        public bool Enable { get; set; }
        public bool EnableAnsiMode { get; set; }
    }

    public class FileLoggingData
    {
        public bool Enable { get; set; } = false;
        public string Path { get; set; } = "";
        public bool EnableLogRotation { get; set; } = false;
        public string RotateLogNameTemplate { get; set; } = "";
    }
}