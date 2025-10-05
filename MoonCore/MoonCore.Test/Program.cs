using System.Text.Json;
using MoonCore.Common;

var x = new List<string>();

for (int i = 0; i < 20; i++)
{
    x.Add($"{i} x");
}

var cd = new CountedData<string>(x, 10324);

Console.WriteLine(cd.Count());

foreach (var a in cd)
{
    Console.WriteLine($"Val: {a}");
}

Console.WriteLine(JsonSerializer.Serialize(cd));