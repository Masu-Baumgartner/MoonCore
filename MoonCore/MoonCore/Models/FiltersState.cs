using MoonCore.Helpers;

namespace MoonCore.Models;

public struct FiltersState
{
    public List<FilterState> Filters { get; set; }
    
    public FiltersState()
    {
        Filters = new();
    }

    public void AddToQuery(QueryStringBuilder builder)
    {
        foreach (var filter in Filters)
        {
            builder.AddOrUpdate($"filters[{filter.Key}]", filter.Value);
            builder.AddOrUpdate($"filters-operation[{filter.Key}]", filter.Type.ToString());
        }
    }

    public static FiltersState FromQuery(QueryStringBuilder builder)
    {
        var result = new FiltersState();
        
        foreach (var queryParam in builder)
        {
            if(!queryParam.Key.StartsWith("filters["))
                continue;

            var keyName = Formatter
                .ReplaceStart(queryParam.Key, "filters[", "")
                .TrimEnd(']');

            var operationKey = $"filters-operation[{keyName}]";

            var filterState = new FilterState();
            
            // Key
            filterState.Key = keyName;
            
            // Value
            filterState.Value = queryParam.Value;
            
            // Operation
            if (!builder.ContainsKey(operationKey))
                filterState.Type = FilterType.Contains;
            else
            {
                if (!Enum.TryParse(builder[operationKey], true, out FilterType filterType))
                    filterState.Type = FilterType.Contains;
                else
                    filterState.Type = filterType;
            }
            
            result.Filters.Add(filterState);
        }

        return result;
    }
}

public struct FilterState
{
    public string Key { get; set; }
    public FilterType Type { get; set; }
    public string Value { get; set; }
}

public enum FilterType
{
    Contains = 0,
    GreaterThan = 1,
    LessThan = 2
}