/*
 *using MoonCore.Helpers.Unix;
using MoonCore.Helpers.Unix.Extensions;

var unixFs = new UnixFileSystem("/var/lib/moonlight/volumes/2");

unixFs.ReadDir("/cache", out _).ThrowIfError();
unixFs.ReadDir("/cache", out _).ThrowIfError();
Console.WriteLine("Removing");
unixFs.RemoveAll("/cache/app").ThrowIfError();
 *
 */

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MoonCore.Configuration;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Models;
using MoonCore.Test;

//var class1 = new Class1()
//{
//    Field2 = "ara"
//};

/*
var class2 = new Class2()
{
    Field1 = "owo",
    Field2 = "o"
};

var class1 = Mapper.Map<Class1>(class2, ignoreNullValues: true);

Console.WriteLine(class1.Field1 ?? "NULL");
Console.WriteLine(class1.Field2 ?? "NULL");*/

/*
var httpClient = new HttpClient();

var response = await httpClient.PostAsync("http://localhost:5165/auth/login", new StringContent("{}",Encoding.UTF8, "application/json"));
var responseText = await response.Content.ReadAsStringAsync();
var errorModel = JsonSerializer.Deserialize<HttpApiErrorModel>(responseText, new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true
});

Console.WriteLine("owo");*/

using var loggerFactory = new LoggerFactory();
loggerFactory.AddProviders(LoggerBuildHelper.BuildFromConfiguration(new MoonCoreLoggingConfiguration()
{
    Console = new()
    {
        Enable = true,
        EnableAnsiMode = true
    },
    FileLogging = new()
    {
        Enable = true,
        Path = "log.txt"
    }
}));

var logger = loggerFactory.CreateLogger<Program>();

for (int i = 0; i < 125; i++)
{
    logger.LogInformation("Owo: " + i);
}