using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoonCore.EnvConfiguration;
using MoonCore.Helpers;
using MoonCore.Test;

var sc = new ServiceCollection();

var configurationBuilder = new ConfigurationBuilder();

configurationBuilder.AddJsonFile(
    PathBuilder.File(Directory.GetCurrentDirectory(), "config.json"),
    optional: true
);

configurationBuilder.AddEnvironmentVariables(prefix: "TESTY_", separator: "_");

var configuration = configurationBuilder.Build();

sc.AddSingleton<IConfiguration>(configuration);

var sp = sc.BuildServiceProvider();

var c = sp.GetRequiredService<IConfiguration>();

var model = c.Get<Model>()!;

var otherModel = sp.GetRequiredService<IOptions<OtherModel>>();

Console.WriteLine(c.GetValue<string>("owo"));
Console.WriteLine(c.GetValue<string>("test"));
Console.WriteLine(c.GetSection("sec1").GetValue<string>("test"));
Console.WriteLine(model.Abc);
Console.WriteLine(otherModel.Value.Other);

Console.WriteLine("---");

foreach (var item in otherModel.Value.Abc)
{
    Console.WriteLine($"A: {item.A}");
}