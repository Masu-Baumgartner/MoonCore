using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Components;

public class InstanceView : IComponent
{
    [Parameter]
    public ComponentBase Instance { get; set; }
    
    private RenderHandle Handle;
    
    public void Attach(RenderHandle renderHandle)
    {
        Handle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        Instance = parameters.GetValueOrDefault<ComponentBase>("Instance", null!);
        
        var fi = GetPrivateField(Instance.GetType(), "_renderFragment");
        var rf = (RenderFragment)fi.GetValue(Instance)!;

        var fi2 = GetPrivateField(Instance.GetType(), "_renderHandle");
        fi2.SetValue(Instance, Handle);
        
        Handle.Render(rf);
        
        return Task.CompletedTask;
    }
    
    private static FieldInfo GetPrivateField(Type t, string name)
    {
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;

        FieldInfo? fi;

        while ((fi = t.GetField(name, bindingFlags)) == null && (t = t.BaseType!) != null)
        {
        }

        return fi!;
    }
}