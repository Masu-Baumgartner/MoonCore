using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace MoonCore.Extensions;

public static class WebHostConfigurationBuilderExtension
{
    public static void ConfigureMoonCoreHttp(this IWebHostBuilder webHostBuilder, int httpPort, bool enableHttps, int httpsPort = 443, X509Certificate2? certificate = default)
    {
        var urls = new List<string>();
        
        urls.Add($"http://0.0.0.0:{httpPort}");
        
        if(enableHttps)
            urls.Add($"https://0.0.0.0:{httpsPort}");

        webHostBuilder.UseUrls(urls.ToArray());

        if (enableHttps)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));
            
            webHostBuilder.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(adapterOptions =>
                {
                    adapterOptions.ServerCertificateSelector = (_, _) => certificate;
                });
            });
        }
    }
}