using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Tailwind.Alerts.Components;

namespace MoonCore.Blazor.Tailwind.Alerts.Interfaces;

public abstract class FormInputUi<T> : ComponentBase, IFormUiEvaluateable<T> where T : class
{
    [CascadingParameter]
    public FormInputModalBase<T> FormInputAlert { get; set; }
    
    /// <summary>
    /// Evaluates the form input and returns an instance of type T if the input is valid.
    /// It should only evaluate the input. If the input is invalid, it should return null, this should be handled by the caller.
    /// </summary>
    public abstract Task<T?> Evaluate();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        
        if (FormInputAlert == null)
        {
            throw new InvalidOperationException("FormInputUi must be a child of FormInputAlert");
        }

        await FormInputAlert.RegisterFormInputUi(this);
    }
}