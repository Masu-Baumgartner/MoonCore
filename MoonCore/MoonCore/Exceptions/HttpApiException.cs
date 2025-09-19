namespace MoonCore.Exceptions;

/// <summary>
/// Represents an RFC 9457 problem detail as an exception
/// Follow <see href="https://www.rfc-editor.org/rfc/rfc9457.html">this</see> link for more details on the RFC
/// </summary>
public class HttpApiException : Exception
{
    /// <summary>
    /// Title of the http error
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Status code of the http error.
    /// Should match the http status code
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> Detail of the http error
    /// </summary>
    public string? Detail { get; set; }
    
    /// <summary>
    /// <b>Optional:</b> List of validation errors
    /// </summary>
    public Dictionary<string, List<string>>? Errors { get; set; }
    
    /// <summary>
    /// Creates an instance of the http api problem details exception
    /// </summary>
    /// <param name="title">Title of the http error</param>
    /// <param name="status">
    /// Status code of the http error.
    /// Should match the http status code
    /// </param>
    /// <param name="detail"><b>Optional:</b> Detail of the http error</param>
    /// <param name="errors"><b>Optional:</b> List of validation errors</param>
    public HttpApiException(string title, int status, string? detail = null, Dictionary<string, List<string>>? errors = null): base($"[{status}] {title}: {detail}")
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }
}