using MoonCore.Test.Modules;

namespace MoonCore.Test;

public class CoolWebService
{
    private readonly MyCoolModule MyCoolModule;

    public CoolWebService(MyCoolModule myCoolModule)
    {
        MyCoolModule = myCoolModule;
    }
}