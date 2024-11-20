namespace MoonCore.Blazor.Tailwind.Alerts.Interfaces;

public interface IFormUiEvaluateable<T> where T : class
{
    public Task<T?> Evaluate();
}