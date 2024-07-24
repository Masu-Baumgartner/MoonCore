using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms;

public interface IFastFormValidator
{
    public ValidationResult? Check(object data);
}