using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.Fast;

public class CustomFastValidator<TProperty> : IFastValidator
{
    private Func<TProperty, ValidationResult?> Func;
    
    public CustomFastValidator(Func<TProperty, ValidationResult?> func)
    {
        Func = func;
    }
    
    public ValidationResult? Check(object data)
    {
        return Func.Invoke((TProperty)data);
    }
}