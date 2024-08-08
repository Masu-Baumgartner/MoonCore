using System.ComponentModel.DataAnnotations;

namespace MoonCore.Helpers;

/// <summary>
/// This class helps you validating a object which class has data annotations
/// </summary>
public class ValidatorHelper
{
    /// <summary>
    /// Validate a object which class has data annotations. Throws a display exception with an error message if not valid
    /// </summary>
    /// <param name="objectToValidate">The object you want to validate</param>
    /// <returns></returns>
    public static bool Validate(object objectToValidate, out string? errorMessage)
    {
        var context = new ValidationContext(objectToValidate, null, null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(objectToValidate, context, results, true);

        if (!isValid)
        {
            var errorMsg = "Unknown form error";

            if (results.Any())
                errorMsg = results.First().ErrorMessage ?? errorMsg;

            errorMessage = errorMsg;
            return false;
        }

        errorMessage = null;
        return true;
    }
    
    /// <summary>
    /// Validates a range of objects which classes have data annotations. Throws a display exception with an error message if not valid
    /// </summary>
    /// <param name="objectToValidate"></param>
    public static bool ValidateRange(IEnumerable<object> objectToValidate, out string? errorMessage)
    {
        foreach (var o in objectToValidate)
        {
            if (Validate(o, out var message))
                continue;

            errorMessage = message;
            return false;
        }

        errorMessage = null;
        return true;
    }
}