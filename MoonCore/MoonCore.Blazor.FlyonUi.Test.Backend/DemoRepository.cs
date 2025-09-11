using Bogus;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;

namespace MoonCore.Blazor.FlyonUi.Test.Backend;

public class DemoRepository
{
    private DemoDataModel[]? Data;

    public IQueryable<DemoDataModel> Get()
    {
        if (Data == null)
        {
            var faker = new Faker<DemoDataModel>();

            Data = faker
                .RuleFor(x => x.Id, f => f.UniqueIndex)
                .RuleFor(x => x.Bool, f => f.Random.Bool())
                .RuleFor(x => x.Number, f => f.Random.Number(1, 10324))
                .RuleFor(x => x.Text, f => f.Name.FullName())
                .GenerateLazy(300)
                .ToArray();
        }

        return Data.AsQueryable();
    }
}