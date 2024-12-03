using MoonCore.Helpers;

namespace MoonCore.Models;

public struct QueryOptions
{
    public List<Filter> Filters { get; set; }
    public List<Sort> Sorting { get; set; }

    public QueryOptions()
    {
        Filters = new();
        Sorting = new();
    }

    public void AddToQuery(QueryStringBuilder builder)
    {
        foreach (var filter in Filters)
        {
            builder.AddOrUpdate($"filter[{filter.Field}]", filter.Value);
            builder.AddOrUpdate($"filter-option[{filter.Field}]", filter.Type.ToString());
        }

        foreach (var sort in Sorting)
        {
            if(sort.Descending)
                builder.AddOrUpdate($"sort[{sort.Field}]", "desc");
            else
                builder.AddOrUpdate($"sort[{sort.Field}]", "asc");
        }
    }

    public static QueryOptions FromQuery(QueryStringBuilder builder)
    {
        var options = new QueryOptions();
        
        foreach (var kvp in builder)
        {
            if (kvp.Key.StartsWith("filter["))
            {
                var field = Formatter
                    .ReplaceStart(kvp.Key, "filter[", "")
                    .TrimEnd(']');

                var value = kvp.Value;
                
                var filter = new Filter()
                {
                    Field = field,
                    Value = value
                };

                var operationKey = $"filter-operation[{field}]";

                if (!builder.ContainsKey(operationKey))
                    filter.Type = FilterType.Contains;
                else if (!Enum.TryParse(builder[operationKey], true, out FilterType filterType))
                    filter.Type = FilterType.Contains;
                else
                    filter.Type = filterType;
                
                options.Filters.Add(filter);
            }
            else if (kvp.Key.StartsWith("sort["))
            {
                var field = Formatter
                    .ReplaceStart(kvp.Key, "sort[", "")
                    .TrimEnd(']');

                var sort = new Sort()
                {
                    Field = field,
                    Descending = kvp.Value.Equals("desc", StringComparison.InvariantCultureIgnoreCase)
                };
                
                options.Sorting.Add(sort);
            }
        }

        return options;
    }
}

public class Filter
{
    public string Field { get; set; }
    public FilterType Type { get; set; }
    public string Value { get; set; }
}

public struct Sort
{
    public string Field { get; set; }
    public bool Descending { get; set; }
}

public enum FilterType
{
    Contains,
    GreaterThen,
    LessThen
}