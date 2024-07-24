using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms.Validators;

public class RegexValidator : IFastFormValidator
{
    private readonly string RegexExpression;
    private readonly string ErrorMessage;

    public RegexValidator(string regexExpression, string errorMessage)
    {
        RegexExpression = regexExpression;
        ErrorMessage = errorMessage;
    }

    public ValidationResult? Check(object data)
    {
        if (data is not string dataAsString)
            throw new ArgumentException("The regex validator can only be used with strings");
        
        if(Regex.IsMatch(dataAsString, RegexExpression))
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }

    public static RegexValidator Create(string regexExpression, string errorMessage) => new(regexExpression, errorMessage);
}