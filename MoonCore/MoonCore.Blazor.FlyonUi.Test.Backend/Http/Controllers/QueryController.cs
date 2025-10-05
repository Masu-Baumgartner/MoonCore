using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;
using MoonCore.Common;
using MoonCore.Models;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[Authorize]
[ApiController]
[Route("api/query")]
public class QueryController : Controller
{
    private readonly DemoRepository Repository;

    public QueryController(DemoRepository repository)
    {
        Repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<CountedData<DemoDataModel>>> Get(
        [FromQuery] int startIndex,
        [FromQuery] int count,
        [FromQuery] string orderBy = "",
        [FromQuery] string orderByDir = "asc",
        [FromQuery] string filter = ""
    )
    {
        var query = Repository.Get();

        query = orderBy switch
        {
            nameof(DemoDataModel.Text) => orderByDir == "desc"
                ? query.OrderByDescending(x => x.Text)
                : query.OrderBy(x => x.Text),
            
            nameof(DemoDataModel.Id) => orderByDir == "desc"
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id),
            
            _ => query
        };

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(x => x.Text.Contains(filter, StringComparison.OrdinalIgnoreCase));

        var totalCount = query
            .Count();
        
        var items = query
            .Skip(startIndex)
            .Take(count)
            .ToArray();

        return new CountedData<DemoDataModel>()
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}