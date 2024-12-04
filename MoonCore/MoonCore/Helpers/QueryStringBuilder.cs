using System.Collections;
using System.Web;

namespace MoonCore.Helpers;

public class QueryStringBuilder : IEnumerable<KeyValuePair<string, string>>
{
    private Dictionary<string, string> Parameters;

    public QueryStringBuilder(string queryString = "")
    {
        Parameters = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(queryString))
            ParseString(queryString);
    }

    // Parses an existing query string and stores the parameters
    private void ParseString(string queryString)
    {
        var queryParts = queryString.TrimStart('?').Split('&');

        foreach (var part in queryParts)
        {
            var keyValue = part.Split('=');

            if (keyValue.Length == 2)
                Parameters[HttpUtility.UrlDecode(keyValue[0])] = HttpUtility.UrlDecode(keyValue[1]);
        }
    }

    // Builds the query string from the parameters
    public string Build()
    {
        if (Parameters.Count == 0)
            return string.Empty;

        return "?" + string.Join("&",
            Parameters.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
    }

    // Adds a new parameter or updates an existing one
    public void AddOrUpdate(string key, string value)
    {
        if (Parameters.ContainsKey(key))
            Parameters[key] = value; // Update
        else
            Parameters.Add(key, value); // Add
    }

    // Removes a parameter by key
    public void Remove(string key)
    {
        if (Parameters.ContainsKey(key))
            Parameters.Remove(key);
    }

    // Retrieves the value of a parameter by key
    public string? Get(string key)
        => Parameters.GetValueOrDefault(key);

    // Updates the value of a parameter by key
    public void Update(string key, string value)
    {
        if (Parameters.ContainsKey(key))
            Parameters[key] = value;
    }

    // To check if the parameter exists
    public bool ContainsKey(string key)
        => Parameters.ContainsKey(key);

    public string? this[string key]
    {
        get => Get(key);
        set => AddOrUpdate(key, value!);
    }

    private static QueryStringBuilder Parse(string queryString)
        => new QueryStringBuilder(queryString);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        => Parameters.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}