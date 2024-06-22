using System.ComponentModel.DataAnnotations;

namespace MoonCore.Blazor.Models.Fast;

public interface IFastValidator
{
    public ValidationResult? Check<T>(T data);
}