using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.Fast.Validators;

public class RequiredValidator : IFastValidator
{
    private string ErrorMessage = "This field is required";
    
    public ValidationResult? Check(object data)
    {
        if (data is string dataAsString)
        {
            if(!string.IsNullOrEmpty(dataAsString))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }

        if(data != null)
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }

    public static RequiredValidator Create(string errorMessage)
    {
        return new RequiredValidator()
        {
            ErrorMessage = errorMessage
        };
    }
}