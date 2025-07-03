using YamlDotNet.Serialization;

namespace MoonCore.Test;

public record TestModel
{
    public DatabaseData Database { get; set; } = new();
    public LimitsData Limits { get; set; } = new();
    public OllamaData Ollama { get; set; } = new();
    public MiscData Misc { get; set; } = new();

    public record OllamaData
    {
        [YamlMember(Description = "The endpoint of your ollama instance. Include protocol and port like 'http://localhost:11434'")]
        public string Endpoint { get; set; } = "http://localhost:11434";
        
        [YamlMember(Description = "The model you want to use for translations. See all models at https://ollama.com/library")]
        public string Model { get; set; } = "gemma3:27b";
    }

    public record DatabaseData
    {
        [YamlMember(Description = "Connection details of your postgresql instance")]
        public string Host { get; set; }
        public int Port { get; set; } = 5432;
        public string Username { get; set; } = "postgres";
        public string Password { get; set; } = "postgres";
        public string Database { get; set; } = "postgres";
    }

    public record LimitsData
    {
        [YamlMember(Description = "The max amount of words allowed to be translated")]
        public int MaxWords { get; set; } = 200;
        
        [YamlMember(Description = "The maximum amount of words until the alternative translations are being disabled")]
        public int AlternativeWords { get; set; } = 10;
    }

    public record MiscData
    {
        [YamlMember(Description = "Enable this when running behind a reverse proxy in order to determine the correct ip address for the logging entries")]
        public bool IsBehindReverseProxy { get; set; } = false;
        
        [YamlMember(Description = "The seconds to wait until starting the translation. This is required in order to allow the client to subscribe to the translation before it starts")]
        public int BackgroundRunDelay { get; set; } = 2;
    }
}