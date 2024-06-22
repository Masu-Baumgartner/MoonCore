using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.Fast;

public class CustomFastValidator<TItem> : IFastValidator where TItem : class
{
    private Func<TItem, ValidationResult?> Func;
    
    public CustomFastValidator(Func<TItem, ValidationResult?> func)
    {
        Func = func;
    }
    
    public ValidationResult? Check<T>(T data)
    {
        return Func.Invoke((data as TItem)!);
    }
}