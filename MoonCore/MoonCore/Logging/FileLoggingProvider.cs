using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class FileLoggingProvider : ILoggerProvider
{
    private readonly StreamWriter StreamWriter;
    private readonly FileStream FileStream;

    public FileLoggingProvider(string path)
    {
        FileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        StreamWriter = new(FileStream);
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, StreamWriter);
    }

    public void Dispose()
    {
        StreamWriter.Flush();
        FileStream.Flush();
        
        StreamWriter.Close();
        FileStream.Close();
    }
}