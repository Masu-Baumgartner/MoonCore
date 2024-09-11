namespace MoonCore.Exceptions;

public class HttpApiException : Exception
{
    public string Title { get; set; } = "";
    public int Status { get; set; } = 0;
    public string Detail { get; set; } = "";
    public Dictionary<string, List<string>> Errors { get; set; } = new();
    
    public HttpApiException(string title, int status, string detail, Dictionary<string, List<string>> errors): base($"[{status}] {title}: {detail}")
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }

    public HttpApiException(string message, string title, int status, string detail, Dictionary<string, List<string>> errors) : base(message)
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }

    public HttpApiException(string message, Exception inner, string title, int status, string detail, Dictionary<string, List<string>> errors) : base(message, inner)
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }
}