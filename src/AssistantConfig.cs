using Discord;
using Newtonsoft.Json;
using System.IO;

namespace Assistant
{
    public class LogConfig
    {
        [JsonProperty("severity")]
        public LogSeverity? Severity { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("ext")]
        public string Ext
        {
            get => Extension;
            set => Extension = value;
        }
    }

    public class ExecutionConfig
    {
        [JsonProperty("memory")]
        public string Memory { get; set; }

        [JsonProperty("cpus")]
        public float CPUs { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }
    }

    public class AssistantConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("successEmote")]
        public string SuccessEmote { get; set; }

        [JsonProperty("failEmote")]
        public string FailEmote { get; set; }

        [JsonProperty("log")]
        public LogConfig Log { get; set; }

        [JsonProperty("snippetPath")]
        public string SnippetPath { get; set; }

        [JsonProperty("exec")]
        public ExecutionConfig Exec { get; set; }

        public static AssistantConfig FromFile(string path) =>
            JsonConvert.DeserializeObject<AssistantConfig>(File.ReadAllText(path));
    }
}
