using Discord;
using Newtonsoft.Json;
using System.IO;

namespace Assistant
{
    public class AssistantConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
        [JsonProperty("logSeverity")]
        public LogSeverity? LogSeverity { get; set; }

        [JsonProperty("logDir")]
        public string LogDir { get; set; }

        public static AssistantConfig FromFile(string path) =>
            JsonConvert.DeserializeObject<AssistantConfig>(File.ReadAllText(path));
    }
}
