using System.Text;
using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class FileLoggingProvider : ILoggerProvider
{
    private readonly TextWriter TextWriter;
    private readonly FileStream FileStream;

    public FileLoggingProvider(string path)
    {
        FileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        TextWriter = new StreamWriter(FileStream, Encoding.UTF8)
        {
            AutoFlush = true
        };
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, TextWriter);
    }

    public void Dispose()
    {
        TextWriter.Flush();
        FileStream.Flush();
        
        TextWriter.Close();
        FileStream.Close();
    }
}