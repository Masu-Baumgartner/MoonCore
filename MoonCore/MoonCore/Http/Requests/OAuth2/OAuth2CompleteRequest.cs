using System.ComponentModel.DataAnnotations;

namespace MoonCore.Http.Requests.OAuth2;

public class OAuth2CompleteRequest
{
    [Required(ErrorMessage = "You need to provide the oauth2 code")]
    public string Code { get; set; }
}