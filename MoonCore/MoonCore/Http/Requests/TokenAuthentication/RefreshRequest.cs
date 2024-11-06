using System.ComponentModel.DataAnnotations;

namespace MoonCore.Http.Requests.TokenAuthentication;

public class RefreshRequest
{
    [Required(ErrorMessage = "You need to provide a refresh token")]
    public string RefreshToken { get; set; }
}