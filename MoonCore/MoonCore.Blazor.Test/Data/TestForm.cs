using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Test.Data;

public class TestForm
{
    [Required(ErrorMessage = "Idk lmao")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Idk lmao email")]
    [EmailAddress]
    public string Email { get; set; }

    public int Ara { get; set; }
}