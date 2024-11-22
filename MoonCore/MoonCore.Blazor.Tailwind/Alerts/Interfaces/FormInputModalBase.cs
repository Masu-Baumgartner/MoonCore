using MoonCore.Blazor.Tailwind.Modals.Components;

namespace MoonCore.Blazor.Tailwind.Alerts.Interfaces;

public abstract class FormInputModalBase<T> : BaseModal where T : class
{
    public abstract Task RegisterFormInputUi(FormInputUi<T> formInputUi);
}