namespace MoonCore.Exceptions;

public class HttpApiException : Exception
{
    public string Title { get; set; }
    public int Status { get; set; } = 500;
    public string? Detail { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; }
    
    public HttpApiException(string title, int status, string? detail = null, Dictionary<string, List<string>>? errors = null): base($"[{status}] {title}: {detail}")
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }
}