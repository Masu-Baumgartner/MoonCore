using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms.Validators;

public class RequiredValidator : IFastFormValidator
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