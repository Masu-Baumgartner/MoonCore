using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Test.Forms;

public class CreateCarForm
{
    [Required]
    public string Name { get; set; }
}