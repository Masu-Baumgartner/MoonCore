using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.Alerts.Interfaces;

public abstract class FormInputUi<T> : ComponentBase, IFormUiEvaluateable<T> where T : class
{
    /// <summary>
    /// Evaluates the form input and returns an instance of type T if the input is valid.
    /// It should only evaluate the input. It cannot not change the state of the component.
    /// </summary>
    public abstract Task<T?> Evaluate();
}