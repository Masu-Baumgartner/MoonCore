using MoonCore.PluginFramework.Test;

var pl = new MyCoolPluginLoader();

pl.Initialize();
pl.HelloWorld();

await pl.Kms();