using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.LegacyForms;

public class InstanceView : IComponent, IHandleAfterRender
{
    [Parameter] public ComponentBase Instance { get; set; }

    private RenderHandle Handle;

    private bool HasBeenInitialized = false;
    private bool IsFirstTime = true;

    private Type InstanceType;

    public void Attach(RenderHandle renderHandle)
    {
        Handle = renderHandle;
    }

    public async Task SetParametersAsync(ParameterView parameters)
    {
        Instance = parameters.GetValueOrDefault<ComponentBase>("Instance", null!);
        InstanceType = Instance.GetType();

        var fi = GetPrivateField(Instance.GetType(), "_renderFragment");
        var rf = (RenderFragment)fi.GetValue(Instance)!;

        var fi2 = GetPrivateField(Instance.GetType(), "_renderHandle");
        fi2.SetValue(Instance, Handle);

        Handle.Render(rf);

        if (!HasBeenInitialized)
        {
            InvokeInstanceMethod("OnInitialized", []);
            await InvokeInstanceMethodAsync("OnInitializedAsync", []);

            HasBeenInitialized = true;
        }
        
        InvokeInstanceMethod("OnParametersSet", []);
        await InvokeInstanceMethodAsync("OnParametersSetAsync", []);
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

    private object? InvokeInstanceMethod(string name, object[] parm)
    {
        return InstanceType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(Instance, parm);
    }

    private async Task InvokeInstanceMethodAsync(string name, object[] parm)
    {
        var resTask = InstanceType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(Instance, parm) as Task;
        await resTask!;
    }

    public async Task OnAfterRenderAsync()
    {
        InvokeInstanceMethod("OnAfterRender", [IsFirstTime]);
        await InvokeInstanceMethodAsync("OnAfterRenderAsync", [IsFirstTime]);
        
        if (IsFirstTime)
            IsFirstTime = false;
    }
}