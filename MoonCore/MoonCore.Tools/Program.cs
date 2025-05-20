using Cocona;
using MoonCore.Tools.Commands;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<TwExtract>();

await app.RunAsync();