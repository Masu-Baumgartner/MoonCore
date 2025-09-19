namespace MoonCore.Models;

/// <summary>
/// Represents an RFC 9457 problem detail as an exception
/// Follow <see href="https://www.rfc-editor.org/rfc/rfc9457.html">this</see> link for more details on the RFC
/// </summary>
public class HttpApiErrorModel
{
    /// <summary>
    /// Title of the http error
    /// </summary>
    public string Title { get; set; } = "";
    
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
}