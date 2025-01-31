using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Tailwind.Fm.Models;

public struct CompressOptionStruct
{
    public CompressType Type { get; set; }
    
    [Required(ErrorMessage = "You need to specify a file name")]
    [RegularExpression("^[^\\\\/:*?\"<>|/]+$", ErrorMessage = "You need to provide a valid file name")]
    public string FileName { get; set; }
}