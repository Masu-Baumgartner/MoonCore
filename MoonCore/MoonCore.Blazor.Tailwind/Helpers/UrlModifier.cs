using System.Collections.Specialized;
using System.Web;

namespace MoonCore.Blazor.Tailwind.Helpers;

public class UrlModifier
{
    public string Scheme { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }

    public string Path { get; set; }
    public NameValueCollection Query { get; set; }

    public UrlModifier(string url)
    {
        var uri = new Uri(url);

        Scheme = uri.Scheme;
        Host = uri.Host;
        Port = uri.Port;
        Path = uri.LocalPath;

        Query = HttpUtility.ParseQueryString(uri.Query);
    }

    public string Build()
    {
        var portStr = Port > 0 ? ":" + Port.ToString() : string.Empty;
        var baseUrl = $"{Scheme}://{Host}{portStr}{Path}";

        var isFirst = true;
        foreach (var queryParam in Query.AllKeys)
        {
            if (isFirst)
            {
                baseUrl += $"?{queryParam}={Query.Get(queryParam)}";
                isFirst = false;
            }
            else
                baseUrl += $"&{queryParam}={Query.Get(queryParam)}";
        }

        return baseUrl;
    }
}