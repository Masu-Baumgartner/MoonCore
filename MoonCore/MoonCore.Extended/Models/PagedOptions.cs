using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Extended.Models;

/// <summary>
/// Defines a paged request loaded via the query
/// </summary>
public class PagedOptions
{
    [FromQuery(Name = "page")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid page specified")]
    public int Page { get; set; }
    
    [FromQuery(Name = "pageSize")]
    [Range(1, 100, ErrorMessage = "Invalid page size: 1-100")]
    public int PageSize { get; set; }
}