﻿namespace MoonCore.Configuration;

public class LoggingConfiguration
{
    public ConsoleData Console { get; set; } = new();
    public FileLoggingData FileLogging { get; set; } = new();

    public class ConsoleData
    {
        public bool Enable { get; set; } = true;
        public bool EnableAnsiMode { get; set; } = true;
    }

    public class FileLoggingData
    {
        public bool Enable { get; set; } = false;
        public string Path { get; set; } = "";
        public bool EnableLogRotation { get; set; } = false;
        public string RotateLogNameTemplate { get; set; } = "";
        public int WriteFlushLimit { get; set; } = 0;
    }
}