using System.ComponentModel.DataAnnotations;
using MoonCore.Exceptions;

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
    /// <exception cref="DisplayException"></exception>
    public static Task Validate(object objectToValidate)
    {
        var context = new ValidationContext(objectToValidate, null, null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(objectToValidate, context, results, true);

        if (!isValid)
        {
            var errorMsg = "Unknown form error";

            if (results.Any())
                errorMsg = results.First().ErrorMessage ?? errorMsg;

            throw new DisplayException(errorMsg);
        }
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Validates a range of objects which classes have data annotations. Throws a display exception with an error message if not valid
    /// </summary>
    /// <param name="objectToValidate"></param>
    public static async Task ValidateRange(IEnumerable<object> objectToValidate)
    {
        foreach (var o in objectToValidate)
            await Validate(o);
    }
}