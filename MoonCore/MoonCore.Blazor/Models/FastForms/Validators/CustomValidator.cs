using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.FastForms.Validators;

public class CustomValidator<T> : IFastFormValidator
{
    public Func<T, ValidationResult?> Func { get; set; }
    
    public ValidationResult? Check(object data)
    {
        return Func.Invoke((T)data);
    }
}