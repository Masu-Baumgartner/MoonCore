using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms.Validators;

public class IntRangeValidator : IFastFormValidator
{
    private readonly int Start;
    private readonly int End;
    private readonly string ErrorMessage;

    public IntRangeValidator(int start, int end, string errorMessage)
    {
        Start = start;
        End = end;
        ErrorMessage = errorMessage;
    }

    public ValidationResult? Check(object data)
    {
        if (data is not int dataAsInt)
            throw new ArgumentException("The regex validator can only be used with ints");
        
        if(dataAsInt >= Start && dataAsInt <= End)
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }

    public static IntRangeValidator Create(int start, int end, string errorMessage) => new(start, end, errorMessage);
}