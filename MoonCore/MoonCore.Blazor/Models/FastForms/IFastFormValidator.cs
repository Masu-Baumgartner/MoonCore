using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.FastForms;

public interface IFastFormValidator
{
    public ValidationResult? Check(object data);
}