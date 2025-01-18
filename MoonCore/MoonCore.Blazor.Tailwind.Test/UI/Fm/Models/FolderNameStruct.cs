using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm.Models;

public class FolderNameStruct
{
    [Required(ErrorMessage = "You need to specify a folder name")]
    [RegularExpression("^[^\\\\/:*?\"<>|/]+$", ErrorMessage = "You need to provide a valid folder name")]
    public string FolderName { get; set; }
}