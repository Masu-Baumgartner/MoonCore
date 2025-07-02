using Microsoft.Extensions.Configuration;
using MoonCore.Test;
using MoonCore.Yaml;

await YamlDefaultGenerator.Generate<TestModel>("config.yml");

var cb = new ConfigurationBuilder();

cb.AddYamlFile("config.yml", prefix: "xyz");

var config = cb.Build();

var model = config.GetSection("xyz").Get<TestModel>();

config.GetHashCode();